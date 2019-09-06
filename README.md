# Speed ORM



Speed.ORM foi desenvolvido com 3 pré-requisitos:

1. Ser o mais rápido ORM no mundo .NET;
2. Produtividade: gerar todo o código Sql, liberando o desenvolvedor para se focar no que interessa;
3. Em alguns minutos, gera a camada de dados e a de negócios, automatizando 90% deste processo. Caberá ao desenvolvedor escrever alguns métodos na camada de negócios.

 

​	Não tem objetivo de ter todos recursos que alguns frameworks têm, tal como o Entity Framework ou NHibernate, mas tem o objetivo de ser muito mais rápido e usar bem menos memória



Download do programa Speed (ClickOnce): https://github.com/carlosast/Speed-ORM/raw/master/publish/setup.exe





## Estrutura do Speed



O Speed é composto de 2 módulos:

1. ### Interface do Usuário

   O Speed possui um software pra gerar todo o código necessário das camadas de dados e negócios
   Não é o foco do Speed definir uma arquitetura da solução. O Speed **gera classes estáticas** para acesso às tabelas e views da base de dados, não sendo necessário ficar criando objetos ou IOC. O desenvolvedor que definirá sua arquitetura e usará o Speed para os dados
   
2. ### Bibliotecas de dados

   Speed é composto por uma DDL principal, "Speed.Data", e uma para cada tipo de base de dados

   1. **Speed.Data**
      Esta DDL possui toda a lógica de acesso a dados:
      1. Classes Database: Classe principal do Speed. Toda chamada à BLL é feita usando a classe Database. Esta classe possui métodos úteis, tais como: ExecuteDatatable, ExecuteScalar, etc.
         Estes são métodos úteis, mas que não são usados pela BLL
   2. **Providers**
      Existe uma DLL para cada tipo de base de dados: Sql Server e Oracle. Estou migrando da versão .NET 4.0 para CORE os providers das seguintes bases de dados: MySql, Firebird, SqlServerCe, PostgreSQL, Access, SQLite e MariaDB



## Como usar o Speed



1. ### Baixar o programa do Speed

   Baixe o programa de geração de dados no link: https://github.com/carlosast/Speed-ORM/raw/master/publish/setup.exe

   Este programa é um ClickOnce.

   Será criado um atalho no desktop com o seguinte ícone: https://github.com/carlosast/Speed-ORM/blob/master/Docs/Speed.png?raw=true

   

   2. Executar o Speed
   2. 

