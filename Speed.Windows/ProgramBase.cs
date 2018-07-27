using System;
using System.Windows.Forms;
using Speed.Data;
using System.ServiceModel;

namespace Speed.Windows
{

    public class ProgramBase : Speed.Data.Sys
    {

        public static string Title = "";
        private const string sep = "--------------------------------------------------------------------------------";
        /// <summary>
        /// Nome da aplicação. Será usado para gravar os parâmetors no registro do Windows
        /// </summary>
        public static string AppName;
        public static bool IsDebug = false;
        /// <summary>
        /// Se usa o Registry 32 bits ou usa o default do OS
        /// </summary>
        public static EnumSource RegistrySource = EnumSource.LocalMachine;
        public static bool UseRegistry32 = false;

        static ProgramBase()
        {

#if DEBUG
            IsDebug = true;
#else
            IsDebug = false;
#endif
        }

        public static void ShowError(string text)
        {
            ShowError(System.Windows.Forms.Form.ActiveForm, text);
        }
        public static void ShowError(Exception ex)
        {
            ShowError(System.Windows.Forms.Form.ActiveForm, Conv.GetErrorMessage(ex, IsDebug));
        }
        public static void ShowError(IWin32Window owner, Exception ex)
        {
            ShowError(owner, Conv.GetErrorMessage(ex, IsDebug));
        }
        public static void ShowError(IWin32Window owner, string text)
        {
            var ctrl = owner as Control;
            if (ctrl != null && ctrl.InvokeRequired)
                ctrl.Invoke(new Action(() => MessageBox.Show(owner, text, Title, MessageBoxButtons.OK, MessageBoxIcon.Error)));
            else
                MessageBox.Show(owner, text, Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void ShowInformation(string text)
        {
            ShowInformation(Form.ActiveForm, text);
        }
        public static void ShowInformation(IWin32Window owner, string text)
        {
            var ctrl = owner as Control;
            if (ctrl != null && ctrl.InvokeRequired)
                ctrl.Invoke(new Action(() => MessageBox.Show(owner, text, Title, MessageBoxButtons.OK, MessageBoxIcon.Information)));
            else
                MessageBox.Show(owner, text, Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static bool Confirm(string text)
        {
            return Confirm(Form.ActiveForm, text);
        }
        public static bool Confirm(Control owner, string text)
        {
            var ctrl = owner as Control;
            if (ctrl != null && ctrl.InvokeRequired)
                return (bool)ctrl.Invoke(new Func<bool>(() => MessageBox.Show(owner != null ? owner.FindForm() : owner, text, Title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes));
            else
                return MessageBox.Show(owner != null ? owner.FindForm() : owner, text, Title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        public static string Prompt(string message, string value)
        {
            InputBoxResult ret = InputBox.Show(message, Title, value);
            if (ret.ReturnCode == DialogResult.OK)
                return ret.Text;
            else
                return value;
        }

        /// <summary>
        /// Runsafe e faz form.Cursor = Cursors.WaitCursor
        /// </summary>
        /// <param name="control"></param>
        /// <param name="action"></param>
        public static bool RunSafe(Control control, Action action)
        {
            bool ret = false;
            control.Cursor = Cursors.WaitCursor;
            control.Update();
            try
            {
                action();
                ret = true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
            control.Cursor = Cursors.Default;
            return ret;
        }

        /// <summary>
        /// Runsafe e faz form.Cursor = Cursors.WaitCursor
        /// </summary>
        /// <param name="control"></param>
        /// <param name="action"></param>
        public static T RunSafe<T>(Control control, Func<T> func)
        {
            control.Cursor = Cursors.WaitCursor;
            control.Update();
            var ret = RunSafe<T>(func);
            control.Cursor = Cursors.Default;
            return ret;
        }

        public static bool RunSafeQuiet(Action action)
        {
            bool ret = false;
            try
            {
                action();
                ret = true;
            }
            catch { }
            return ret;
        }

        public static bool RunSafeInDb(Control control, Action<Database> action)
        {
            if (control != null)
            {
                control.Cursor = Cursors.WaitCursor;
                control.Update();
            }
            bool ret = false;
            try
            {
                using (var db = NewDb())
                    action(db);
                ret = true;
            }
            catch (Exception ex)
            {
                if (control != null) control.Cursor = Cursors.Default;
                ShowError(ex);
            }
            if (control != null) control.Cursor = Cursors.Default;
            return ret;
        }

        #region Transaction

        /// <summary>
        /// Método útil pra executar um comando dentro de uma transaction, usando uma base de dados já criada
        /// A transaction é iniciada em RunInTran. Se sucesso, fará um Commit. Se não, Roolback
        /// </summary>
        /// <param name="db"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public new static bool RunInTran(Database db, Action action)
        {
            try
            {
                db.BeginTransaction();
                action();
                db.Commit();
                return true;
            }
            catch (Exception ex)
            {
                db.Rollback();
                return false;
            }
        }

        public new static bool RunSafeInTran(Action<Database> action)
        {
            bool ret = false;
            try
            {
                using (var db = NewDb())
                {
                    db.BeginTransaction();
                    action(db);
                    db.Commit();
                }
                ret = true;
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
            return ret;
        }

        public new static void RunInTran(Action<Database> action)
        {
            using (var db = NewDb())
            {
                db.BeginTransaction();
                action(db);
                db.Commit();
            }
        }

        #endregion Transaction

        #region IsValid

        /// <summary>
        /// Método útil para ser usado nos retornos de chamadas ao WCF.
        /// Já faz tratamento de erro
        /// </summary>
        /// <param name="error"></param>
        /// <param name="cancelled"></param>
        /// <returns></returns>
        public static bool IsValid(Exception error, bool cancelled)
        {
            return IsValid(error, cancelled, true);
        }

        /// <summary>
        /// Método útil para ser usado nos retornos de chamadas ao WCF.
        /// Já faz tratamento de erro
        /// </summary>
        /// <param name="error"></param>
        /// <param name="cancelled"></param>
        /// <returns></returns>
        public static bool IsValid(Exception error, bool cancelled, bool showError)
        {
            if (error != null)
            {
                var fault = error as FaultException<ExceptionDetail>;
                if (showError)
                {
                    if (fault != null)
                        ShowError(fault);
                    else
                        ShowError(error);
                }
                return false;
            }
            else if (cancelled)
            {
                if (showError) ShowInformation("Operação cancelada.");
                return false;
            }
            return true;
        }

        public static bool IsValid(object _event)
        {
            return IsValid(_event, true);
        }

        public static bool IsValid(object _event, bool showError)
        {
            return IsValid((Exception)ReflectionUtil.GetProperty(_event, "Error"), (bool)ReflectionUtil.GetProperty(_event, "Cancelled"), showError);
        }

        #endregion IsValid

        /// <summary>
        /// Checa se o registro do windows foi configurado e, se sim, seta a ConnectionString
        /// </summary>
        /// <param name="showDialog">Se exibe FormRegister, caso o registro não tenha sido configurado</param>
        public static bool CheckRegister(bool showDialog)
        {
            bool ret = false;
            try
            {
                using (var reg = new RegUtil(RegistrySource, AppName))
                {
                    reg.Check();

                    if (!reg.IsOk)
                    {
                        if (showDialog)
                        {
                            using (var f = new FormRegister())
                            {
                                if (f.ShowDialog() != DialogResult.OK)
                                    return false;
                            }
                        }
                    }

                    if (!reg.IsOk)
                        throw new Exception("O sistema não foi configurado corretamente");

                    ConnectionString = Database.BuildConnectionString(ProviderType, reg.Server, reg.Database, reg.UserId, reg.Password, false, reg.Port).ConnectionString;

                    ret = true;
                }
            }
            catch (Exception ex)
            {
                if (showDialog)
                    ShowError(ex);
                else
                    throw ex;
            }
            return ret;
        }

        public static bool ShowDialogRegister()
        {
            using (var f = new FormRegister())
            {
                return (f.ShowDialog() == DialogResult.OK);
            }
        }

    }

}
