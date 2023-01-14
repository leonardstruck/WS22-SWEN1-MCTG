drop table if exists battle;

drop table if exists session;

drop table if exists trading;

drop table if exists card;

drop table if exists package;

drop table if exists "user";


create table "user"
(
    id               uuid    default gen_random_uuid() not null
        constraint user_pk
            primary key,
    username         varchar,
    password         varchar,
    coins            integer,
    name             varchar,
    bio              varchar,
    image            varchar,
    elo              integer default 1500              not null,
    wins             integer default 0                 not null,
    losses           integer default 0                 not null,
    lobby_entry      timestamp,
    reward_timestamp timestamp
);

create table battle
(
    id         uuid default gen_random_uuid() not null
        constraint battle_pk
            primary key,
    opponent_1 uuid                           not null
        constraint battle_user_opp1_fk
            references "user",
    opponent_2 uuid                           not null
        constraint battle_user_opp2_fk
            references "user",
    timestamp  timestamp,
    log        jsonb
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
    id          uuid    default gen_random_uuid() not null
        constraint card_pk
            primary key,
    package_id  uuid                              not null
        constraint card_package_fk
            references package,
    name        varchar,
    damage      numeric,
    owner_id    uuid
        constraint card_user_id_fk
            references "user",
    "inDeck"    boolean default false             not null,
    "tradeLock" boolean default false             not null,
    constraint "check_inDeck_notTradeable"
        check ((("inDeck" IS TRUE) AND ("tradeLock" IS FALSE)) OR ("inDeck" IS FALSE))
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

create table trading
(
    id          uuid default gen_random_uuid() not null
        constraint trading_pk
            primary key,
    card_id     uuid                           not null
        constraint trading_card_fk
            references card,
    "minDamage" integer                        not null,
    type        varchar                        not null
);

