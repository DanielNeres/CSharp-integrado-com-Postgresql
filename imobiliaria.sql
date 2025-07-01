create table inquilino (
	id_inquilino serial primary key,
	nome text not null,
	cpf char(15) unique not null,
	email text unique not null,
	telefone_1 char(20) unique not null,
	telefone_2 char(20),
	data_nascimento date not null
);

create table proprietario (
	id_proprietario serial primary key,
	nome text not null,
	cpf char(15) unique not null,
	email text unique not null,
	telefone_1 char(20) unique not null,
	telefone_2 char(20)
);

create table imovel (
	id_imovel serial primary key,
	id_proprietario integer not null,
	valor_aluguel decimal(11,2) not null,
	status boolean not null,
	numero integer not null,
	rua text not null,
	bairro text not null,
	cidade text not null,
	foreign key (id_proprietario) references proprietario on update cascade on delete cascade
);

create table corretor (
	id_corretor serial primary key,
	nome text not null,
	creci char(11) unique not null,
	email text unique not null,
	telefone_1 char(20) unique not null,
	telefone_2 char(20)
);

create table aluguel(
	id_aluguel serial primary key,
	id_inquilino integer not null,
	id_imovel integer not null,
	id_corretor integer not null,
	data_inicio date not null,
	data_fim date not null,
	foreign key (id_inquilino) references inquilino on update cascade on delete cascade,
	foreign key (id_imovel) references imovel on update cascade on delete cascade,
	foreign key (id_corretor) references corretor on update cascade on delete cascade
);