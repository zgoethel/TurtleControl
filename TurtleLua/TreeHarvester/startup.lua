Routine = require("routine")

while true do
    Routine.waitFor("minecraft:oak_log")
    Routine.harvestTree()
    Routine.returnHome()

    Routine.placeSapling()
    sleep(5)
    Routine.collectItems()

    Routine.returnHome2()
    Routine.dumpItems()
    Routine.returnHome3()
                  
    print("Harvest cycle complete")
end
