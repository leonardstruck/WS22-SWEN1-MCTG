drop table if exists card;

drop table if exists package;

drop table if exists "user";

create table "user"
(
    id       serial
        constraint user_pk
            primary key,
    username varchar,
    password varchar,
    coins    integer,
    name     varchar,
    bio      varchar,
    image    varchar
);

create table package
(
    id          serial
        constraint package_pk
            primary key,
    acquired_by integer
        constraint package_user_fk
            references "user"
);

create table card
(
    id         uuid                  not null
        constraint card_pk
            primary key,
    package_id integer               not null
        constraint card_package_fk
            references package,
    name       varchar,
    damage     numeric,
    owner_id   integer
        constraint card_user_id_fk
            references "user",
    "inDeck"   boolean default false not null
);



