CREATE TABLE IF NOT EXISTS public.tag
(
    id integer NOT NULL,
    name text COLLATE pg_catalog."default",
    count integer,
    CONSTRAINT tag_pkey PRIMARY KEY (id)
)