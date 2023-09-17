CREATE PROCEDURE Event_AddMaterial
	@Id INT,
	@Material VARCHAR(50),
	@Amount INT
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE EventMaterial SET
		NetAmount = NetAmount + @Amount,
		Transactions = Transactions + 1,
		LastUpdated = GETDATE()
	WHERE EventId = @Id
		AND Material = @Material

	IF @@ROWCOUNT = 0
	BEGIN
		INSERT INTO EventMaterial (
			EventId,
			Material,
			NetAmount,
			Transactions)
		VALUES (
			@Id,
			@Material,
			@Amount,
			1)
	END
END
