FROM postgres:16
COPY 01_schema.sql /docker-entrypoint-initdb.d/
COPY 02_triggers.sql /docker-entrypoint-initdb.d/
COPY 03_sample-data.sql /docker-entrypoint-initdb.d/
EXPOSE 5432

