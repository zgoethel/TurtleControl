local Routine = {}
local Event = require("event")
local Inventory = require("inventory")

function Routine.checkFuel()
    if turtle.getFuelLevel() >= 4 then
        return
    end
    local event = Event.begin("Refuel")

    while turtle.getFuelLevel() < 30 do
        local slotInfo = turtle.getItemDetail(
            turtle.getSelectedSlot())
        if slotInfo and turtle.refuel(1) then
            Event.addMaterial(event, slotInfo.name, -1)
        end
    end
end

--local firstRun = true

function Routine.waitFor(block)
    --if not firstRun then
    --    sleep(380)
    --end
    --firstRun = false

    while true do
        --local event = Event.begin("CheckFor '" .. block .. "'")
        
        --Routine.checkFuel()
        --local remember = Inventory.deepCopy()
        --turtle.dig()
        --Inventory.logDiffs(event, remember)

        --Routine.checkFuel()
        --turtle.forward()
        
        local success, found = turtle.inspect()
        if success and found.name == block then
            break
        end

        Routine.checkFuel()
        --turtle.back()
        sleep(1)--380)
    end
end

function Routine.harvestTree()
    local event = Event.begin("HarvestTree")

    Routine.checkFuel()
    local remember = Inventory.deepCopy()
    turtle.dig()
    Inventory.logDiffs(event, remember)
    
    Routine.checkFuel()
    turtle.forward()

    while true do
        success, b = turtle.inspectUp()
        if success
            and b.name == "minecraft:stone"
        then
            break
        end

        Routine.checkFuel()
        remember = Inventory.deepCopy()
        turtle.digUp()
        Inventory.logDiffs(event, remember)

        Routine.checkFuel()
        turtle.up()
    end
end

function Routine.returnHome()
    Routine.checkFuel()
    while turtle.down() do
        Routine.checkFuel()
    end

    Routine.checkFuel()
    turtle.back()
end

function Routine.collectItems()
    --TODO Configure turtles in database
    -- 2
    if os.getComputerID() == 2 then
        turtle.turnLeft()
    -- 4
    else
        turtle.turnRight()
    end

    for i = 1, 4, 1 do
        Routine.checkFuel()
        turtle.forward()
    end

    for i = 1, 2, 1 do
        Routine.checkFuel()
        turtle.down()
    end

    turtle.turnLeft()
    turtle.turnLeft()

    local event = Event.begin("VacuumSuck")
    local remember = Inventory.deepCopy()
    while turtle.suck() do
        Inventory.logDiffs(event, remember)
        remember = Inventory.deepCopy()
    end
end

function Routine.returnHome2()
    for i = 1, 2, 1 do
        Routine.checkFuel()
        turtle.up()
    end

    turtle.turnLeft()
    turtle.turnLeft()
end

function Routine.dumpItems(firstSlot)
    for i = firstSlot, 16, 1 do
        turtle.select(i)
        while turtle.drop() do
        end
    end
    turtle.select(1)
end

function Routine.returnHome3()
    turtle.turnLeft()
    turtle.turnLeft()
    
    for i = 1, 4, 1 do
        Routine.checkFuel()
        turtle.forward()
    end
    
    --TODO Configure turtles in database
    -- 2
    if os.getComputerID() == 2 then
        turtle.turnLeft()
    -- 4
    else
        turtle.turnRight()
    end

    --Routine.checkFuel()
    --turtle.back()
end

function Routine.placeBlock(slot)
    local event = Event.begin("PlaceBlock")
    Routine.checkFuel()
    turtle.select(slot)

    local remember = Inventory.deepCopy()
    turtle.place()
    Inventory.logDiffs(event, remember)

    turtle.select(1)
end

return Routine