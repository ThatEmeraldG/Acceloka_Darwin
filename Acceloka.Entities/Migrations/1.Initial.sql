create database acceloka;

select current_database();

create table Ticket(
	ticket_code VARCHAR(10) primary key not null,
	ticket_name VARCHAR(255) not null,
	category_id INT not null,
	event_start TIMESTAMP not null,
	event_end TIMESTAMP not null,
	quota INT not null,
	price INT not null,
	created_at TIMESTAMP default CURRENT_TIMESTAMP not null,
	updated_at TIMESTAMP null,
	created_by VARCHAR(50) not null,
	updated_by VARCHAR(50) null,
	foreign key (category_id) references Category(category_id) on delete cascade
);

create table Category(
	category_id SERIAL primary key,
	category_name VARCHAR(50) not null unique,
	created_at TIMESTAMP default CURRENT_TIMESTAMP not null,
	updated_at TIMESTAMP null,
	created_by VARCHAR(50) not null,
	updated_by VARCHAR(50) null
);

create table Users (
	user_id VARCHAR(10) primary key not null,
	user_name VARCHAR(50) not null,
	user_email VARCHAR(255) not null unique,
	user_password VARCHAR(255) not null,
	created_at TIMESTAMP default CURRENT_TIMESTAMP not null,
	updated_at TIMESTAMP null
)

create table "Transaction" (
    transaction_id SERIAL primary key,
    transaction_date TIMESTAMP default CURRENT_TIMESTAMP,
    total_price INT not null,
    total_payment INT not null,
    payment_method VARCHAR(50) not null,
	created_at TIMESTAMP default CURRENT_TIMESTAMP not null,
	updated_at TIMESTAMP null,
	created_by VARCHAR(50) not null,
	updated_by VARCHAR(50) null
)

create table BookedTicket (
	booked_ticket_id SERIAL primary key,
	transaction_id INT not null,
    booking_date TIMESTAMP default CURRENT_TIMESTAMP,
    total_category_price INT not null,
	created_at TIMESTAMP default CURRENT_TIMESTAMP not null,
	updated_at TIMESTAMP null,
	created_by VARCHAR(50) not null,
	updated_by VARCHAR(50) null,
    foreign key (transaction_id) references Transaction(transaction_id) on delete cascade
);

create table BookedTicketDetail (
    booked_detail_id SERIAL primary key,
    booked_ticket_id INT not null,
    ticket_code VARCHAR(10) not null,
    ticket_quantity INT not null,
    total_ticket_price INT not null,
	created_at TIMESTAMP default CURRENT_TIMESTAMP not null,
	updated_at TIMESTAMP null,
	created_by VARCHAR(50) not null,
	updated_by VARCHAR(50) null,
    foreign key (booked_ticket_id) references BookedTicket(booked_ticket_id) on delete cascade,
    foreign key (ticket_code) references Ticket(ticket_code) on delete cascade
);
