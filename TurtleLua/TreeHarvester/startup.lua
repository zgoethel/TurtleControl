Routine = require("routine")

while true do
    Routine.waitFor("minecraft:oak_log")
    Routine.harvestTree()
    Routine.returnHome()

    sleep(5)
    Routine.placeBlock(2)
    Routine.collectItems()

    Routine.returnHome2()
    Routine.dumpItems(3)
    Routine.returnHome3()
                  
    print("Harvest cycle complete")
    sleep(10)
end
