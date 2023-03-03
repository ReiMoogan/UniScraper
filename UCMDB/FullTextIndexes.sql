CREATE FULLTEXT INDEX ON [UCM].[professor]
    ([last_name] LANGUAGE 1033, [first_name] LANGUAGE 1033, [email] LANGUAGE 1033)
    KEY INDEX [professor_pk]
    ON [ClassCatalogue];


GO
CREATE FULLTEXT INDEX ON [UCM].[class]
    ([course_number] LANGUAGE 1033, [course_title] LANGUAGE 1033)
    KEY INDEX [class_id_uindex]
    ON [ClassCatalogue];

