ALTER TABLE dbo.Tales ADD CONSTRAINT
	DF_Tales_CreatedOn DEFAULT (sysdatetimeoffset()) FOR CreatedOn
GO