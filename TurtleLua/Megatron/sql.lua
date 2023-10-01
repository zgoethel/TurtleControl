local Sql = {}
local Api = require("api")
if not json then
    os.loadAPI("json")
end

local area = "Sql"

function Sql.exec(proc, pars)
    local path = area .. "/Exec"
    return Api.post(path, {
        proc = proc,
        pars = pars
    })
end

function Sql.execJson(proc, pars)
    local path = area .. "/ExecJson"
    return Api.postJson(path, {
        proc = proc,
        pars = pars
    })
end

return Sql
