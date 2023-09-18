local Inventory = {}
local Event = require("event")

function Inventory.deepCopy()
    local copy = {}
    
    for slot = 1, 16, 1 do
        local itemDetail = turtle.getItemDetail(slot)
        if itemDetail then
            local copyItem = {
                name = itemDetail.name,
                count = itemDetail.count
            }
            table.insert(copy, copyItem)
        else
            table.insert(copy, nil)
        end
    end
    
    return copy
end

function insertMatCount(matCounts, slot, sub)
    if not slot then
        return
    end

    if not matCounts[slot.name] then
        matCounts[slot.name] = { new = 0, old = 0 }
    end
    matCounts[slot.name][sub] = matCounts[slot.name][sub] + slot.count
end

function Inventory.logDiffs(event, original)
    matCounts = {}
    
    for slot = 1, 16, 1 do
        local newSlot = turtle.getItemDetail(slot)
        insertMatCount(matCounts, newSlot, "new")

        local oldSlot = original[slot]
        insertMatCount(matCounts, oldSlot, "old")
    end

    for material, counts in pairs(matCounts) do
        local diff = counts.new - counts.old
        if diff ~= 0 then
            Event.addMaterial(event, material, diff)
        end
    end
end

return Inventory