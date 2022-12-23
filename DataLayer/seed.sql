drop table if exists card;

drop table if exists package;

drop table if exists "session";

drop table if exists "user";


create table "user"
(
    id       uuid    default gen_random_uuid() not null
        constraint user_pk
            primary key,
    username varchar,
    password varchar,
    coins    integer,
    name     varchar,
    bio      varchar,
    image    varchar,
    elo      integer default 1500              not null,
    wins     integer default 0                 not null,
    losses   integer default 0                 not null
);

create table package
(
    id          uuid default gen_random_uuid() not null
        constraint package_pk
            primary key,
    acquired_by uuid
        constraint package_user_fk
            references "user"
);

create table card
(
    id         uuid    default gen_random_uuid() not null
        constraint card_pk
            primary key,
    package_id uuid                              not null
        constraint card_package_fk
            references package,
    name       varchar,
    damage     numeric,
    owner_id   uuid
        constraint card_user_id_fk
            references "user",
    "inDeck"   boolean default false             not null
);

create table session
(
    id      uuid      default gen_random_uuid() not null
        constraint sessions_pk
            primary key,
    token   varchar                             not null
        constraint session_pk
            unique,
    expires timestamp default (now() + '24:00:00'::interval),
    user_id uuid                                not null
        constraint session_user_id_fk
            references "user"
);

