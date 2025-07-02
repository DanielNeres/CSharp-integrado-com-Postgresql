# CSharp-PostgreSQL-Imobiliaria

![C#](https://img.shields.io/badge/C%23-PostgreSQL-blueviolet?style=for-the-badge&logo=c-sharp&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-Database-blue?style=for-the-badge&logo=postgresql&logoColor=white)

Este projeto é um sistema simples de gerenciamento de imobiliária desenvolvido em C#, que interage com um banco de dados PostgreSQL. Ele oferece um menu interativo para realizar operações de **criação e leitura** em entidades como Inquilinos, Imóveis, Aluguéis, Proprietários e Corretores.

##  Tecnologias Utilizadas

* **C#**: Linguagem de programação principal.
* **PostgreSQL**: Sistema de gerenciamento de banco de dados relacional.
* **Npgsql**: Provedor de dados .NET para PostgreSQL.

##  Estrutura do Banco de Dados

O banco de dados é composto por cinco tabelas principais: `inquilino`, `proprietario`, `imovel`, `corretor` e `aluguel`.
* A tabela `inquilino` armazena informações pessoais dos inquilinos, incluindo nome, CPF, email, telefones e data de nascimento.
* A tabela `proprietario` armazena dados de proprietários, como nome, CPF, email e telefones.
* A tabela `imovel` detalha os imóveis, com informações como valor do aluguel, status, endereço e uma chave estrangeira para o proprietário.
* A tabela `corretor` contém dados dos corretores, incluindo nome, CRECI, email e telefones.
* A tabela `aluguel` registra os contratos de aluguel, conectando inquilinos, imóveis e corretores através de chaves estrangeiras, além de datas de início e fim.

##  Funcionalidades

O sistema apresenta um menu principal que dá acesso a submenus específicos para cada entidade, permitindo as operações:

### Menu Principal
* Gerenciamento de Inquilinos, Imóveis, Aluguéis, Proprietários e Corretores.

### Menu Inquilino
* Inserir novo inquilino.
* Consultar inquilino por nome.
* Exibir todos os inquilinos.
* Consultar inquilinos com o 2º telefone preenchido.

### Menu Imóvel
* Inserir Imóvel.
* Consultar Imóvel por Rua ou Bairro.
* Exibir todos os Imóveis.
* Filtrar Imóvel pelo valor do aluguel.
* Consultar o primeiro Imóvel de uma cidade.
* Listar todas as cidades cadastradas.

### Menu Aluguel
* Inserir Aluguel.
* Consultar Aluguel por Nome do Inquilino.
* Exibir todos os Aluguéis.

### Menu Proprietário
* Inserir Proprietário.
* Consultar Proprietário por Nome.
* Exibir todos os Proprietários.

### Menu Corretor
* Inserir Corretor.
* Consultar Corretor por Nome.
* Exibir todos os Corretores.

##  Como Configurar e Rodar

Para configurar e executar este projeto, siga os passos abaixo:

### 1. Configurar o PostgreSQL
* Certifique-se de ter o PostgreSQL instalado e em execução.
* Crie um banco de dados (ex: "NomeDoBanco").
* Execute o script SQL fornecido (`imobiliaria.sql`) para criar as tabelas necessárias no seu banco de dados.

### 2. Configurar o Projeto C#
Antes de configurar a string de conexão, prepare seu ambiente C#:
* Navegue até o diretório principal do projeto no terminal.
* Adicione o pacote Npgsql ao seu projeto:
    ```bash
    dotnet add package Npgsql
    ```
* Se o projeto ainda não estiver inicializado como um projeto de console, execute:
    ```bash
    dotnet new console
    ```
    *(Nota: Este comando criará um novo projeto de console com um `Program.cs` e um arquivo `.csproj`. Se você já tem os arquivos `CsBanco.cs` e `README.md` no diretório, certifique-se de que o `CsBanco.cs` seja o ponto de entrada ou de que o `Program.cs` faça referência a ele, ou renomeie `CsBanco.cs` para `Program.cs`.)*

Agora, configure a conexão com o banco de dados:
* Abra o arquivo `CsBanco.cs`.
* Localize a `connectionString` e atualize-a com suas credenciais do PostgreSQL (Host, Porta, Usuário, Senha e Nome do Banco de Dados).

    ```csharp
    static string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=SUA_SENHA_AQUI;Database=NomeDoBanco";
    ```
    -   A porta **5432** e o username **postgres** definidos nesse comando de conexão são os do padrão de configuração do PostgreSQL. Se você definiu algo diferente, atualize essas informações.
    -   Substitua `SUA_SENHA_AQUI` pela sua senha de usuário `postgres`.

### 3. Executar o Aplicativo
* Compile e execute o projeto C#. Você pode usar um IDE como o Visual Studio ou via linha de comando com o .NET SDK:

    ```bash
    dotnet run
    ```
