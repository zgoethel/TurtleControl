local Event = {}
local Sql = require("sql")

function getCCType()
    if turtle then return "Turtle"
    elseif pocket then return "Pocket"
    elseif commands then return "CommandComputer"
    else return "Computer"
    end
end

function Event.begin(type)
    local result = Sql.execJson("Event_Begin",
        {
            Type = type,
            CCType = getCCType(),
            CCNum = os.getComputerID()
        })
    if result and #(result) > 0 then
        return result[1].Id
    else
        return nil
    end
end

function Event.addMaterial(id, material, amount)
    local result = Sql.exec("Event_AddMaterial",
        {
            Id = id,
            Material = material,
            Amount = amount
        })
    return result
end

return Event
