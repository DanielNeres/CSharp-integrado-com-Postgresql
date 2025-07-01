using System;
using Npgsql;


class Banco{
    static string connectionString = "Host=localhost;Port=5432;Username=postgres;Password=**************;Database=NomeDoBanco";

    static void Main(){
        Menu();
    }

    static void AguardarEntrada(){
        Console.WriteLine("\nPressione qualquer tecla para continuar...");
        Console.ReadKey();

    }


    static void Menu(){

        while (true){
            Console.Clear();
            Console.WriteLine("===== MENU PRINCIPAL =====");
            Console.WriteLine("1 - Inquilinos");
            Console.WriteLine("2 - Imóveis");
            Console.WriteLine("3 - Aluguéis");
            Console.WriteLine("4 - Proprietários");
            Console.WriteLine("5 - Corretores");
            Console.WriteLine("0 - Sair");
            Console.Write("Escolha uma opção: ");
            string opcao = Console.ReadLine() ?? "0";
            switch (opcao){
                case "1":
                    MenuInquilino();
                    break;

                case "2":
                    MenuImovel();
                    break;

                case "3":

                    MenuAluguel();
                    break;

                case "4":

                    MenuProprietario();
                    break;

                case "5":

                    MenuCorretor();
                    break;

                case "0":

                    Console.WriteLine("Saindo...");
                    Environment.Exit(0);
                    return;

                default:

                    Console.WriteLine("Opção inválida. Tente novamente.");
                    AguardarEntrada();
                    break;
            }
        }
    }


    static void MenuInquilino(){

        while (true){

            Console.Clear();
            Console.WriteLine("===== MENU INQUILINO =====");
            Console.WriteLine("1 - Inserir novo inquilino");
            Console.WriteLine("2 - Consultar inquilino por nome");
            Console.WriteLine("3 - Exibir todos os inquilinos");
            Console.WriteLine("4 - Consultar inquilino com o 2 telefone preenchido");
            Console.WriteLine("0 - Voltar ao Menu Principal");
            Console.Write("Escolha uma opção: ");
            string opcao = Console.ReadLine() ?? "0";

            switch (opcao){
                case "1": InquilinoInserir(); break;
                case "2": InquilinoConsultarPorNome(); break;
                case "3": InquilinoExibir(); break;
                case "4": InquilinoTelefone2Preenchido(); break;
                case "0": return;

                default:
                    Console.WriteLine("Opção inválida. Tente novamente.");
                    break;
            }
            AguardarEntrada();
        }
    }


    static void InquilinoInserir(){

        Console.WriteLine("\n--- Inserir Inquilino ---");
        Console.Write("Nome: ");
        string nome = Console.ReadLine() ?? "";
        Console.Write("CPF: ");
        string cpf = Console.ReadLine() ?? "";
        Console.Write("Email: ");
        string email = Console.ReadLine() ?? "";
        Console.Write("Telefone 1: ");
        string telefone1 = Console.ReadLine() ?? "";
        Console.Write("Telefone 2 (pressione Enter se não tiver): ");
        string telefone2 = Console.ReadLine() ?? "";
        Console.Write("Data de nascimento (AAAA-MM-DD): ");
        string dataStr = Console.ReadLine() ?? "";
        DateTime dataNascimento = DateTime.Parse(dataStr);



        using (var conn = new NpgsqlConnection(connectionString)){

            conn.Open();

            string sql = @"INSERT INTO inquilino (nome, cpf, email, telefone_1, telefone_2, data_nascimento)
            VALUES (@nome, @cpf, @email, @telefone1, @telefone2, @dataNascimento);";

            using (var cmd = new NpgsqlCommand(sql, conn)){

                cmd.Parameters.AddWithValue("nome", nome);
                cmd.Parameters.AddWithValue("cpf", cpf);
                cmd.Parameters.AddWithValue("email", email);
                cmd.Parameters.AddWithValue("telefone1", telefone1);
                cmd.Parameters.AddWithValue("telefone2", string.IsNullOrWhiteSpace(telefone2) ? DBNull.Value : telefone2);
                cmd.Parameters.AddWithValue("dataNascimento", dataNascimento);

                cmd.ExecuteNonQuery();
                Console.WriteLine("Inquilino inserido com sucesso!");

            }
        }
    }


    static void InquilinoConsultarPorNome(){

        Console.Clear();
        Console.WriteLine("\n--- Consultar Inquilino ---");
        Console.Write("Digite o nome (ou parte): ");
        string nomeBusca = Console.ReadLine() ?? "";

        using (var conn = new NpgsqlConnection(connectionString)){

            conn.Open();

            string sql = @"SELECT id_inquilino, nome, cpf, email, telefone_1, telefone_2, data_nascimento
            FROM inquilino WHERE nome ILIKE @nome;";

            using (var cmd = new NpgsqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("nome", $"%{nomeBusca}%");

                using (var reader = cmd.ExecuteReader())
                {

                    if (!reader.HasRows)
                    {
                        Console.WriteLine("Nenhum inquilino encontrado.");
                        return;
                    }

                    Console.WriteLine("\n--- Resultados ---");
                    while (reader.Read()){
                        Console.WriteLine(
                          $"ID: {reader.GetInt32(0)} | Nome: {reader.GetString(1)} | CPF: {reader.GetString(2)}\n" +
                          $"Email: {reader.GetString(3)} | Tel 1: {reader.GetString(4)} | Tel 2: {(reader.IsDBNull(5) ? "null" : reader.GetString(5))} | " +
                          $"Nascimento: {reader.GetDateTime(6):dd/MM/yyyy}\n"
                        );
                    }
                }
            }
        }
    }



    static void InquilinoExibir(){


        using (var conn = new NpgsqlConnection(connectionString)){

            conn.Open();

            string sql = @"SELECT * FROM public.inquilino
            ORDER BY id_inquilino ASC";

            using (var cmd = new NpgsqlCommand(sql, conn)){

                using (var reader = cmd.ExecuteReader()){

                    if (!reader.HasRows){

                        Console.WriteLine("Nenhum inquilino encontrado.");
                        return;
                    }

                    Console.WriteLine("\n--- Resultados ---");
                    while (reader.Read()){

                        Console.WriteLine(
                          $"ID: {reader.GetInt32(0)} | Nome: {reader.GetString(1)} | CPF: {reader.GetString(2)}\n" +
                          $"Email: {reader.GetString(3)} | Tel 1: {reader.GetString(4)} | Tel 2: {(reader.IsDBNull(5) ? "null" : reader.GetString(5))} | " +
                          $"Nascimento: {reader.GetDateTime(6):dd/MM/yyyy}\n"
                        );
                    }
                }
            }
        }
    }
    

    static void InquilinoTelefone2Preenchido(){
        Console.WriteLine("\n--- Inquilinos com Telefone 2 Preenchido ---");

        using (var conn = new NpgsqlConnection(connectionString)){
            conn.Open();

            string sql = @"
                SELECT id_inquilino, nome, cpf, email, telefone_1, telefone_2, data_nascimento
                FROM inquilino
                WHERE telefone_2 IS NOT NULL
                ORDER BY id_inquilino ASC;
            ";

            using (var cmd = new NpgsqlCommand(sql, conn)){
                using (var reader = cmd.ExecuteReader()){
                    if (!reader.HasRows){
                        Console.WriteLine("Nenhum inquilino com Telefone 2 preenchido.");
                        return;
                    }

                    Console.WriteLine("\n--- Resultados ---");
                    while (reader.Read()){
                        Console.WriteLine(
                            $"ID: {reader.GetInt32(0)} | Nome: {reader.GetString(1)} | CPF: {reader.GetString(2)}\n" +
                            $"Email: {reader.GetString(3)} | Tel 1: {reader.GetString(4)} | Tel 2: {reader.GetString(5)} | " +
                            $"Nascimento: {reader.GetDateTime(6):dd/MM/yyyy}\n"
                        );
                    }
                }
            }
        }
    }



    static void MenuImovel(){

        while (true){

            Console.Clear();
            Console.WriteLine("1 - Inserir Imóvel");
            Console.WriteLine("2 - Consultar Imóvel por Rua ou Bairro");
            Console.WriteLine("3 - Exibir todos os Imóveis");
            Console.WriteLine("4 - Consultar Imóvel pelo valor do aluguel");
            Console.WriteLine("5 - Consultar primeiro Imóvel de uma cidade");
            Console.WriteLine("6 - Listar todas as cidades");
            Console.WriteLine("0 - Voltar");

            Console.Write("Escolha uma opção: ");
            string opcao = Console.ReadLine() ?? "0";

            switch (opcao){

                case "1": ImovelInserir(); break;
                case "2": ImovelConsultarPorRuaOuBairro(); break;
                case "3": ImovelExibir(); break;
                case "4": ImovelFiltrarPorValorAluguel(); break;
                case "5": ImovelPrimeiroPorCidade(); break;
                case "6": ImovelListarCidades(); break;
                case "0": return;

                default: Console.WriteLine("Opção inválida."); break;

            }
            AguardarEntrada();
        }
    }



    static void ImovelInserir(){

        Console.WriteLine("\n--- Inserir Imovel ---");
        Console.Write("id_proprietario: ");
        int id_proprietario = int.Parse(Console.ReadLine() ?? "0");
        Console.Write("valor_aluguel: ");
        decimal valor = decimal.Parse(Console.ReadLine() ?? "0");
        Console.Write("status (true/false): ");
        bool status = bool.Parse(Console.ReadLine() ?? "false");
        Console.Write("numero: ");
        int numero = int.Parse(Console.ReadLine() ?? "0");
        Console.Write("rua: ");
        string rua = Console.ReadLine() ?? "";
        Console.Write("bairro: ");
        string bairro = Console.ReadLine() ?? "";
        Console.Write("cidade: ");
        string cidade = Console.ReadLine() ?? "";

        using (var conn = new NpgsqlConnection(connectionString)){

            conn.Open();



            string sql = @"INSERT INTO imovel (id_proprietario, valor_aluguel, status, numero, rua, bairro, cidade)
            VALUES (@id, @valor, @status, @numero, @rua, @bairro, @cidade)";

            using (var cmd = new NpgsqlCommand(sql, conn)){

                cmd.Parameters.AddWithValue("id", id_proprietario);
                cmd.Parameters.AddWithValue("valor", valor);
                cmd.Parameters.AddWithValue("status", status);
                cmd.Parameters.AddWithValue("numero", numero);
                cmd.Parameters.AddWithValue("rua", rua);
                cmd.Parameters.AddWithValue("bairro", bairro);
                cmd.Parameters.AddWithValue("cidade", cidade);

                cmd.ExecuteNonQuery();
                Console.WriteLine("Imovel inserido com sucesso!");
            }
        }
    }


    static void ImovelConsultarPorRuaOuBairro(){

        Console.WriteLine("\n--- Consultar Imóvel ---");
        Console.Write("Digite a rua ou bairro (ou parte): ");
        string busca = Console.ReadLine() ?? "";


        using (var conn = new NpgsqlConnection(connectionString)){

            conn.Open();

            string sql = @"SELECT id_imovel, id_proprietario, valor_aluguel, status, numero, rua, bairro, cidade
            FROM imovel WHERE rua ILIKE @busca OR bairro ILIKE @busca;";

            using (var cmd = new NpgsqlCommand(sql, conn)){

                cmd.Parameters.AddWithValue("busca", $"%{busca}%");
                using (var reader = cmd.ExecuteReader()){

                    if (!reader.HasRows){

                        Console.WriteLine("Nenhum imóvel encontrado.");
                        return;
                    }

                    Console.WriteLine("\n--- Resultados ---");
                    while (reader.Read()){

                        Console.WriteLine(
                          $"ID: {reader.GetInt32(0)} | ID Proprietário: {reader.GetInt32(1)} | Valor: R$ {reader.GetDecimal(2):F2} | " +
                          $"Status: {(reader.GetBoolean(3) ? "Disponível" : "Indisponível")}\n" +
                          $"Endereço: {reader.GetString(5)}, Nº {reader.GetInt32(4)} - {reader.GetString(6)}, {reader.GetString(7)}\n"
                        );
                    }
                }
            }
        }
    }



    static void ImovelExibir(){

        using (var conn = new NpgsqlConnection(connectionString)){

            conn.Open();

            string sql = @"SELECT * FROM public.imovel
            ORDER BY id_imovel ASC";



            using (var cmd = new NpgsqlCommand(sql, conn)){
                using (var reader = cmd.ExecuteReader()){
                    if (!reader.HasRows){
                        Console.WriteLine("Nenhum imóvel encontrado.");
                        return;

                    }

                    Console.WriteLine("\n--- Resultados ---");
                    while (reader.Read()){

                        Console.WriteLine(
                          $"ID: {reader.GetInt32(0)} | ID Proprietário: {reader.GetInt32(1)} | " +
                          $"Valor Aluguel: R$ {reader.GetDecimal(2):F2} | Status: {(reader.GetBoolean(3) ? "Disponível" : "Indisponível")}\n" +
                          $"Número: {reader.GetInt32(4)} | Rua: {reader.GetString(5)} | Bairro: {reader.GetString(6)} | Cidade: {reader.GetString(7)}\n"
                        );
                    }
                }
            }
        }
    }
    
    static void ImovelFiltrarPorValorAluguel(){
        Console.WriteLine("\n--- Filtrar Imóveis por Faixa de Aluguel ---");

        Console.Write("Valor mínimo: R$ ");
        decimal valorMin = decimal.Parse(Console.ReadLine() ?? "0");

        Console.Write("Valor máximo: R$ ");
        decimal valorMax = decimal.Parse(Console.ReadLine() ?? "0");

        using (var conn = new NpgsqlConnection(connectionString)){
            conn.Open();

            string sql = @"
                SELECT id_imovel, id_proprietario, valor_aluguel, status, numero, rua, bairro, cidade
                FROM imovel
                WHERE valor_aluguel BETWEEN @min AND @max
                ORDER BY valor_aluguel ASC;
            ";

            using (var cmd = new NpgsqlCommand(sql, conn)){
                cmd.Parameters.AddWithValue("min", valorMin);
                cmd.Parameters.AddWithValue("max", valorMax);

                using (var reader = cmd.ExecuteReader()){
                    if (!reader.HasRows){
                        Console.WriteLine("Nenhum imóvel encontrado dentro da faixa de aluguel especificada.");
                        return;
                    }

                    Console.WriteLine("\n--- Resultados ---");
                    while (reader.Read()){
                        Console.WriteLine(
                            $"ID: {reader.GetInt32(0)} | ID Proprietário: {reader.GetInt32(1)} | " +
                            $"Valor Aluguel: R$ {reader.GetDecimal(2):F2} | Status: {(reader.GetBoolean(3) ? "Disponível" : "Indisponível")}\n" +
                            $"Endereço: {reader.GetString(5)}, Nº {reader.GetInt32(4)} - {reader.GetString(6)}, {reader.GetString(7)}\n"
                        );
                    }
                }
            }
        }
    }

    static void ImovelPrimeiroPorCidade(){
        Console.WriteLine("\n--- Buscar o Primeiro Imóvel de uma Cidade ---");
        Console.Write("Digite o nome da cidade: ");
        string cidade = Console.ReadLine() ?? "";

        using (var conn = new NpgsqlConnection(connectionString)){
            conn.Open();

            string sql = @"
                SELECT id_imovel, id_proprietario, valor_aluguel, status, numero, rua, bairro, cidade
                FROM imovel
                WHERE cidade ILIKE @cidade
                ORDER BY id_imovel ASC
                LIMIT 1;
            ";

            using (var cmd = new NpgsqlCommand(sql, conn)){
                cmd.Parameters.AddWithValue("cidade", cidade);

                using (var reader = cmd.ExecuteReader()){
                    if (!reader.HasRows){
                        Console.WriteLine("Nenhum imóvel encontrado nessa cidade.");
                        return;
                    }

                    reader.Read(); // só vai ter um, pois usamos LIMIT 1

                    Console.WriteLine("\n--- Imóvel Encontrado ---");
                    Console.WriteLine(
                        $"ID: {reader.GetInt32(0)} | ID Proprietário: {reader.GetInt32(1)} | " +
                        $"Valor Aluguel: R$ {reader.GetDecimal(2):F2} | Status: {(reader.GetBoolean(3) ? "Disponível" : "Indisponível")}\n" +
                        $"Endereço: {reader.GetString(5)}, Nº {reader.GetInt32(4)} - {reader.GetString(6)}, {reader.GetString(7)}\n"
                    );
                }
            }
        }
    }

    static void ImovelListarCidades(){
        Console.WriteLine("\n--- Cidades com Imóveis Cadastrados ---");

        using (var conn = new NpgsqlConnection(connectionString)){
            conn.Open();
            string sql = @"
                SELECT DISTINCT cidade
                FROM imovel
                ORDER BY cidade ASC;
            ";

            using (var cmd = new NpgsqlCommand(sql, conn)){
                using (var reader = cmd.ExecuteReader()){
                    if (!reader.HasRows){
                        Console.WriteLine("Nenhuma cidade encontrada na tabela de imóveis.");
                        return;
                    }

                    Console.WriteLine("\n--- Cidades ---");
                    while (reader.Read()){
                        Console.WriteLine($"- {reader.GetString(0)}");
                    }
                }
            }
        }
    }


    static void MenuAluguel(){

        while (true){

            Console.Clear();
            Console.WriteLine("\n===== MENU ALUGUEL =====");
            Console.WriteLine("1 - Inserir Aluguel");
            Console.WriteLine("2 - Consultar Aluguel por Nome do Inquilino");
            Console.WriteLine("3 - Exibir todos os Aluguéis");
            Console.WriteLine("0 - Voltar");

            Console.Write("Escolha uma opção: ");
            string opcao = Console.ReadLine() ?? "0";

            switch (opcao)
            {

                case "1": AluguelInserir(); break;
                case "2": AluguelConsultarPorNomeInquilino(); break;
                case "3": AluguelExibir(); break;
                case "0": return;
                default: Console.WriteLine("Opção inválida."); break;

            }
            AguardarEntrada();
        }
    }

    static void AluguelInserir(){

        Console.WriteLine("\n--- Inserir Aluguel ---");
        Console.Write("ID do Inquilino: ");
        int idInquilino = int.Parse(Console.ReadLine() ?? "0");
        Console.Write("ID do Imóvel: ");
        int idImovel = int.Parse(Console.ReadLine() ?? "0");
        Console.Write("ID do Corretor: ");
        int idCorretor = int.Parse(Console.ReadLine() ?? "0");
        Console.Write("Data de Início (AAAA-MM-DD): ");
        string dataInicioStr = Console.ReadLine() ?? "";
        DateTime dataInicio = DateTime.Parse(dataInicioStr);

        Console.Write("Data de Fim (AAAA-MM-DD): ");
        string dataFimStr = Console.ReadLine() ?? "";
        DateTime dataFim = DateTime.Parse(dataFimStr);

        using (var conn = new NpgsqlConnection(connectionString)){

            conn.Open();
            string sql = @"INSERT INTO aluguel (id_inquilino, id_imovel, id_corretor, data_inicio, data_fim)
            VALUES (@idInquilino, @idImovel, @idCorretor, @dataInicio, @dataFim);";

            using (var cmd = new NpgsqlCommand(sql, conn)){
                cmd.Parameters.AddWithValue("idInquilino", idInquilino);
                cmd.Parameters.AddWithValue("idImovel", idImovel);
                cmd.Parameters.AddWithValue("idCorretor", idCorretor);
                cmd.Parameters.AddWithValue("dataInicio", dataInicio);
                cmd.Parameters.AddWithValue("dataFim", dataFim);

                cmd.ExecuteNonQuery();
                Console.WriteLine("Aluguel inserido com sucesso!");
            }
        }
    }

    static void AluguelConsultarPorNomeInquilino(){

        Console.WriteLine("\n--- Consultar Aluguel por Nome do Inquilino ---");
        Console.Write("Digite o nome (ou parte): ");
        string nomeBusca = Console.ReadLine() ?? "";

        using (var conn = new NpgsqlConnection(connectionString)){

            conn.Open();

            string sql = @"SELECT
            a.id_aluguel,
            i.nome AS nome_inquilino,
            im.id_imovel,
            c.nome AS nome_corretor,
            a.data_inicio,a.data_fim
            FROM aluguel a
            JOIN inquilino i ON a.id_inquilino = i.id_inquilino
            JOIN imovel im ON a.id_imovel = im.id_imovel
            JOIN corretor c ON a.id_corretor = c.id_corretor
            WHERE i.nome ILIKE @nome;";

            using (var cmd = new NpgsqlCommand(sql, conn)){

                cmd.Parameters.AddWithValue("nome", $"%{nomeBusca}%");

                using (var reader = cmd.ExecuteReader()){

                    if (!reader.HasRows){
                        Console.WriteLine("Nenhum aluguel encontrado.");
                        return;

                    }

                    Console.WriteLine("\n--- Resultados ---");
                    while (reader.Read()){

                        Console.WriteLine(
                          $"ID Aluguel: {reader.GetInt32(0)} | Inquilino: {reader.GetString(1)} | Imóvel ID: {reader.GetInt32(2)}\n" +
                          $"Corretor: {reader.GetString(3)} | Início: {reader.GetDateTime(4):dd/MM/yyyy} | Fim: {reader.GetDateTime(5):dd/MM/yyyy}\n"
                        );
                    }
                }
            }
        }
    }



    static void AluguelExibir(){

        using (var conn = new NpgsqlConnection(connectionString)){

            conn.Open();

            string sql = @"SELECT
            a.id_aluguel,
            i.nome AS nome_inquilino,
            im.id_imovel,
            c.nome AS nome_corretor,
            a.data_inicio,
            a.data_fim
            FROM aluguel a
            JOIN inquilino i ON a.id_inquilino = i.id_inquilino
            JOIN imovel im ON a.id_imovel = im.id_imovel
            JOIN corretor c ON a.id_corretor = c.id_corretor
            ORDER BY a.id_aluguel ASC;";

            using (var cmd = new NpgsqlCommand(sql, conn)){
                using (var reader = cmd.ExecuteReader()){
                    if (!reader.HasRows){

                        Console.WriteLine("Nenhum aluguel encontrado.");
                        return;

                    }

                    Console.WriteLine("\n--- Resultados ---");
                    while (reader.Read()){

                        Console.WriteLine(
                          $"ID Aluguel: {reader.GetInt32(0)} | Inquilino: {reader.GetString(1)} | Imóvel ID: {reader.GetInt32(2)}\n" +
                          $"Corretor: {reader.GetString(3)} | Início: {reader.GetDateTime(4):dd/MM/yyyy} | Fim: {reader.GetDateTime(5):dd/MM/yyyy}\n"
                        );
                    }
                }
            }
        }
    }


    static void MenuProprietario()
    {

        while (true)
        {

            Console.Clear();
            Console.WriteLine("\n===== MENU PROPRIETÁRIO =====");
            Console.WriteLine("1 - Inserir Proprietário");
            Console.WriteLine("2 - Consultar Proprietário por Nome");
            Console.WriteLine("3 - Exibir todos os Proprietários");
            Console.WriteLine("0 - Voltar");

            Console.Write("Escolha uma opção: ");
            string opcao = Console.ReadLine() ?? "0";

            switch (opcao)
            {

                case "1": ProprietarioInserir(); break;
                case "2": ProprietarioConsultarPorNome(); break;
                case "3": ProprietarioExibir(); break;
                case "0": return;
                default: Console.WriteLine("Opção inválida."); break;
            }
            AguardarEntrada();
        }
    }



    static void ProprietarioInserir(){

        Console.WriteLine("\n--- Inserir Proprietário ---");
        Console.Write("Nome: ");
        string nome = Console.ReadLine() ?? "";
        Console.Write("CPF: ");
        string cpf = Console.ReadLine() ?? "";
        Console.Write("Email: ");
        string email = Console.ReadLine() ?? "";
        Console.Write("Telefone 1: ");
        string telefone1 = Console.ReadLine() ?? "";
        Console.Write("Telefone 2 (pressione Enter se não tiver): ");
        string telefone2 = Console.ReadLine() ?? "";

        using (var conn = new NpgsqlConnection(connectionString)){

            conn.Open();
            string sql = @"INSERT INTO proprietario (nome, cpf, email, telefone_1, telefone_2)
            VALUES (@nome, @cpf, @email, @telefone1, @telefone2);";

            using (var cmd = new NpgsqlCommand(sql, conn)){

                cmd.Parameters.AddWithValue("nome", nome);
                cmd.Parameters.AddWithValue("cpf", cpf);
                cmd.Parameters.AddWithValue("email", email);
                cmd.Parameters.AddWithValue("telefone1", telefone1);
                cmd.Parameters.AddWithValue("telefone2", string.IsNullOrWhiteSpace(telefone2) ? DBNull.Value : telefone2);

                cmd.ExecuteNonQuery();
                Console.WriteLine("Proprietário inserido com sucesso!");
            }
        }
    }


    static void ProprietarioConsultarPorNome(){

        Console.WriteLine("\n--- Consultar Proprietário por Nome ---");
        Console.Write("Digite o nome (ou parte): ");
        string nomeBusca = Console.ReadLine() ?? "";

        using (var conn = new NpgsqlConnection(connectionString)){

            conn.Open();

            string sql = @"SELECT id_proprietario, nome, cpf, email, telefone_1, telefone_2
            FROM proprietario
            WHERE nome ILIKE @nome;";

            using (var cmd = new NpgsqlCommand(sql, conn)){

                cmd.Parameters.AddWithValue("nome", $"%{nomeBusca}%");
                using (var reader = cmd.ExecuteReader()){

                    if (!reader.HasRows){
                        Console.WriteLine("Nenhum proprietário encontrado.");
                        return;

                    }

                    Console.WriteLine("\n--- Resultados ---");
                    while (reader.Read()){

                        Console.WriteLine(
                          $"ID: {reader.GetInt32(0)} | Nome: {reader.GetString(1)} | CPF: {reader.GetString(2)}\n" +
                          $"Email: {reader.GetString(3)} | Telefone 1: {reader.GetString(4)} | Telefone 2: {(reader.IsDBNull(5) ? "Não informado" : reader.GetString(5))}\n"
                        );
                    }
                }
            }
        }
    }

    static void ProprietarioExibir(){

        using (var conn = new NpgsqlConnection(connectionString)){

            conn.Open();
            string sql = @"SELECT * FROM public.proprietario
            ORDER BY id_proprietario ASC";

            using (var cmd = new NpgsqlCommand(sql, conn)){
                using (var reader = cmd.ExecuteReader()){
                    if (!reader.HasRows){
                        Console.WriteLine("Nenhum proprietário encontrado.");
                        return;
                    }

                    Console.WriteLine("\n--- Resultados ---");
                    while (reader.Read()){
                        Console.WriteLine(
                        $"ID: {reader.GetInt32(0)} | Nome: {reader.GetString(1)} | CPF: {reader.GetString(2)}\n" +
                        $"Email: {reader.GetString(3)} | Tel 1: {reader.GetString(4)} | Tel 2: {(reader.IsDBNull(5) ? "null" : reader.GetString(5))}\n"
                        );
                    }
                }
            }
        }
    }


    static void MenuCorretor(){
        while (true){

            Console.Clear();
            Console.WriteLine("\n===== MENU CORRETOR =====");
            Console.WriteLine("1 - Inserir Corretor");
            Console.WriteLine("2 - Consultar Corretor por Nome");
            Console.WriteLine("3 - Exibir todos os Corretores");
            Console.WriteLine("0 - Voltar");
            Console.Write("Escolha uma opção: ");
            string opcao = Console.ReadLine() ?? "0";
            switch (opcao){

                case "1": CorretorInserir(); break;
                case "2": CorretorConsultarPorNome(); break;
                case "3": CorretorExibir(); break;
                case "0": return;
                default: Console.WriteLine("Opção inválida."); break;
            }
            AguardarEntrada();
        }
    }

    static void CorretorInserir(){
        Console.WriteLine("\n--- Inserir Corretor ---");
        Console.Write("Nome: ");
        string nome = Console.ReadLine() ?? "";
        Console.Write("CRECI: ");
        string creci = Console.ReadLine() ?? "";
        Console.Write("Email: ");
        string email = Console.ReadLine() ?? "";
        Console.Write("Telefone 1: ");
        string telefone1 = Console.ReadLine() ?? "";
        Console.Write("Telefone 2 (pressione Enter se não tiver): ");
        string telefone2 = Console.ReadLine() ?? "";
        using (var conn = new NpgsqlConnection(connectionString)){
            conn.Open();
            string sql = @"INSERT INTO corretor (nome, creci, email, telefone_1, telefone_2)
            VALUES (@nome, @creci, @email, @telefone1, @telefone2);";


            using (var cmd = new NpgsqlCommand(sql, conn)){

                cmd.Parameters.AddWithValue("nome", nome);
                cmd.Parameters.AddWithValue("creci", creci);
                cmd.Parameters.AddWithValue("email", email);
                cmd.Parameters.AddWithValue("telefone1", telefone1);
                cmd.Parameters.AddWithValue("telefone2", string.IsNullOrWhiteSpace(telefone2) ? DBNull.Value : telefone2);

                cmd.ExecuteNonQuery();
                Console.WriteLine("Corretor inserido com sucesso!");

            }
        }
    }


    static void CorretorConsultarPorNome(){

        Console.WriteLine("\n--- Consultar Corretor por Nome ---");
        Console.Write("Digite o nome (ou parte): ");
        string nomeBusca = Console.ReadLine() ?? "";
        using (var conn = new NpgsqlConnection(connectionString)){

            conn.Open();
            string sql = @"SELECT id_corretor, nome, creci, email, telefone_1, telefone_2
            FROM corretor WHERE nome ILIKE @nome;";

            using (var cmd = new NpgsqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("nome", $"%{nomeBusca}%");
                using (var reader = cmd.ExecuteReader()){
                    if (!reader.HasRows){
                        Console.WriteLine("Nenhum corretor encontrado.");
                        return;

                    }

                    Console.WriteLine("\n--- Resultados ---");
                    while (reader.Read()){

                        Console.WriteLine(
                        $"ID: {reader.GetInt32(0)} | Nome: {reader.GetString(1)} | CRECI: {reader.GetString(2)}\n" +
                        $"Email: {reader.GetString(3)} | Telefone 1: {reader.GetString(4)} | Telefone 2: {(reader.IsDBNull(5) ? "Não informado" : reader.GetString(5))}\n"
                        );
                    }
                }
            }
        }
    }





    static void CorretorExibir(){

        using (var conn = new NpgsqlConnection(connectionString)){

            conn.Open();
            string sql = @"SELECT * FROM public.corretor ORDER BY id_corretor ASC";

            using (var cmd = new NpgsqlCommand(sql, conn)){
                using (var reader = cmd.ExecuteReader()){
                    if (!reader.HasRows){
                        Console.WriteLine("Nenhum corretor encontrado.");
                        return;
                    }

                    Console.WriteLine("\n--- Resultados ---");
                    while (reader.Read()){
                        Console.WriteLine(
                          $"ID: {reader.GetInt32(0)} | Nome: {reader.GetString(1)} | CRECI: {reader.GetString(2)}\n" +
                          $"Email: {reader.GetString(3)} | Tel 1: {reader.GetString(4)} | Tel 2: {(reader.IsDBNull(5) ? "null" : reader.GetString(5))}\n"
                        );
                    }
                }
            }
        }
    }
}