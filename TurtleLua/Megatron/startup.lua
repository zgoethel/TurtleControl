local Sql = require("sql")

function noDec(int_val)
    return tostring(int_val):gsub("%.0", "")
end

while true do
    local monitor = peripheral.wrap("bottom")
    monitor.setTextScale(2.5)
    monitor.setCursorPos(1, 1)
    
    local _dashboard = Sql.execJson("Event_Dashboard", { })
    if _dashboard and _dashboard[1] then
       local dashboard = _dashboard[1]
        
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
