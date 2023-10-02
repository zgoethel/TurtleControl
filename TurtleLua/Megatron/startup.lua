local Sql = require("sql")
local Left = peripheral.wrap("left")
local Right = peripheral.wrap("right")
local Top = peripheral.wrap("top")

function noDec(int_val)
    return tostring(int_val):gsub("%.0", "")
end

local dashboard = {TurtleLeaderboard = {}}
local oldDashboard = {TurtleLeaderboard = {}}

while true do
    local monitor = peripheral.wrap("bottom")
    monitor.setTextScale(2.5)
    monitor.setCursorPos(1, 1)
    
    local _dashboard = Sql.execJson("Event_Dashboard", { })
    if _dashboard and _dashboard[1] then
        oldDashboard = dashboard
      dashboard = _dashboard[1]
        
    for idx = 1, math.min(#(dashboard.TurtleLeaderboard), #(oldDashboard.TurtleLeaderboard)), 1 do
            if dashboard.TurtleLeaderboard[idx].LastTree ~=
                oldDashboard.TurtleLeaderboard[idx].LastTree
            then
                --Left.playSound("block.chest.open", 0.7)
                --sleep(0.3)
                Right.playNote("harp", 3, 1)
                Left.playNote("bit", 3, 6)
                sleep(0.13)
                Right.playNote("harp", 3, 1)
                Left.playNote("bit", 3, 6)
                sleep(0.13)
                Right.playNote("harp", 3, 6)
                Left.playNote("bit", 3, 10)
                Top.playNote("bit", 3, 13)
                sleep(0.13)
                Top.playNote("bit", 3, 1)
                Right.playNote("bass", 3, 1)
                sleep(0.13)
                Right.playNote("bass", 8, 4)
                Left.playNote("xylophone", 3, 1)
                Top.playNote("bit", 3, 3)
                sleep(0.13)
                Right.playNote("bass", 3, 1)
                Top.playNote("bit", 3, 5)
                sleep(0.13)
                Left.playNote("xylophone", 3, 10)
                --Right.playNote("bass", 3, 6)
                local randomness = math.random(0, 6);
                if randomness == 0 then
                    Right.playSound("entity.endermite.hurt", 3)
                elseif randomness == 1 then
                    Right.playSound("entity.enderman.hurt", 3)
                elseif randomness == 2 then
                    Right.playSound("block.bell.use", 3)
                elseif randomness == 3 then
                    Right.playSound("entity.enderman.death", 2)
                elseif randomness == 4 then
                    Right.playSound("entity.wolf.ambient", 3)
                elseif randomness == 5 then
                    Right.playSound("entity.ocelot.ambient", 3)
                elseif randomness == 6 then
                    Right.playSound("entity.panda.pre_sneeze", 3)
                    sleep(0.4)
                    Right.playSound("entity.panda.sneeze", 3)
                end
                Top.playNote("bit", 3, 6)
                sleep(0.13 * 2)
                Left.playNote("xylophone", 3, 9)
                Right.playNote("bass", 3, 5)
                Top.playNote("bit", 3, 9)
                sleep(0.13 * 2)
                Left.playNote("xylophone", 3, 8)
                Right.playNote("bass", 3, 4)
                Top.playNote("bit", 3, 8)

                sleep(0.13 * 2)
                Left.playNote("xylophone", 3, 10)
                Right.playNote("bass", 3, 3)
                Top.playNote("bit", 3, 6)
            end
    end
        
    monitor.clear()
    monitor.write(noDec(dashboard.HarvestedTrees))
    monitor.write(" trees")
    
    monitor.setCursorPos(1, 2)
    monitor.write("+")
    monitor.write(noDec(dashboard.HarvestedLogs))
    monitor.write(" logs")
    
    monitor.setCursorPos(1, 3)
    monitor.write(noDec(dashboard.FuelConsumed))
    monitor.write(" fuel")

    monitor.setCursorPos(1, 4)
    monitor.write("============")
    monitor.setCursorPos(1, 5)
    --monitor.write("#")
    monitor.write(noDec(dashboard.TurtleLeaderboard[1].CCNum))
    monitor.write(" is ahead")   
       
    else
        monitor.setCursorPos(1, 1)
        monitor.write("Net error")
    end         
     sleep(1)    
end
