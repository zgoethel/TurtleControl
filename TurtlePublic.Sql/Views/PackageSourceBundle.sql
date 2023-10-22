CREATE VIEW [dbo].[PackageSourceBundle] AS
SELECT p.Id,
    p.Name,
    ps.IsDirty,
    sv.Path,
    ISNULL(ps.VersionId, latest.Id) 'VersionId',
    sv.Source,
    sv.Committed,
    sv.CommittedBy
FROM Package p
INNER JOIN PackageSource ps ON ps.PackageId = p.Id
OUTER APPLY (
    SELECT TOP (1) Id FROM SourceVersion _sv
    WHERE _sv.Path = ps.Path
    ORDER BY _sv.Id DESC) AS [latest]
LEFT JOIN SourceVersion sv ON sv.Id = ISNULL(ps.VersionId, latest.Id)