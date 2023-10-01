local Api = {}
if not json then
    os.loadAPI("json")
end

local base = "http://winvm-a.jibini.net/TurtleControl/"
local headers = { ['Content-Type'] = "application/json" }

function Api.post(path, body)
    local result, b, c
    local bodyJson = json.encode(body)
    bodyJson = string.gsub(bodyJson, "^%{\"pars\":%[%],", "{\"pars\":{},")
     
    local fullPath = base .. path
    while true do
        result, b, c = http.post(fullPath, bodyJson, headers)

        if b ~= "Could not connect"
            and b ~= "Service Unavailable"
        then
            break
        end
        print(b)
        
        sleep(2)
        print("Retrying connection to '" .. fullPath .. "'")
    end

    if result then
        result.close()
        return true
    else
        print(b)
        print(c)
        return false
    end
end

function Api.postJson(path, body)
    local result, b, c
    local bodyJson = json.encode(body)
    bodyJson = string.gsub(bodyJson, "^%{\"pars\":%[%],", "{\"pars\":{},")
    
    local fullPath = base .. path
    while true do
        result, b, c = http.post(fullPath, bodyJson, headers)

        if b ~= "Could not connect"
            and b ~= "Service Unavailable"
        then
            break
        end
        print(b)

        sleep(2)
        print("Retrying connection to '" .. fullPath .. "'")
    end

    if result then
        local content = result.readAll()
        result.close()
        return json.decode(content)
    else
        print(b)
        print(c)
        return nil
    end
end

return Api
