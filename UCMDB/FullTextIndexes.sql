CREATE FULLTEXT INDEX ON [UCM].[professor]
    ([full_name] LANGUAGE 1033)
    KEY INDEX [PK_professor]
    ON [ClassCatalogue];


GO
CREATE FULLTEXT INDEX ON [UCM].[class]
    ([course_number] LANGUAGE 1033, [course_title] LANGUAGE 1033)
    KEY INDEX [PK_class]
    ON [ClassCatalogue];

