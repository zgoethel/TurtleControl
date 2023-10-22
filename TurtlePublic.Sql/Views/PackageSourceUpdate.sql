CREATE VIEW [dbo].[PackageSourceUpdate] AS
SELECT tp.TurtleId,
    psb.Id 'PackageId',
    psb.Name,
    psb.Path,
    psb.IsDirty,
    psb.VersionId,
    psb.Source,
    psb.Committed,
    psb.CommittedBy
FROM PackageSourceBundle psb
INNER JOIN TurtlePackage tp ON psb.Id = tp.PackageId
WHERE IsDirty = 1
    OR tp.Published IS NULL