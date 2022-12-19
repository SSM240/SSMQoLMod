local debugUtils = require("debug_utils")
local notifications = require("ui.notification")
local snapshot = require("structs.snapshot")
local state = require("loaded_state")

local script = {
    name = "sortRooms",
    displayName = "Sort Rooms",
    tooltip = "Sorts all rooms in alphabetical order"
}

function script.prerun(args)
    local oldRoomOrder = {}
    for k,v in ipairs(state.map.rooms) do
        oldRoomOrder[v.name] = k
    end

    local function forward(data)
        table.sort(state.map.rooms, function(room1, room2) return room1.name < room2.name end)
        debugUtils.reloadUI()
    end
    
    local function backward(data)
        table.sort(state.map.rooms, function(room1, room2) return oldRoomOrder[room1.name] < oldRoomOrder[room2.name] end)
        debugUtils.reloadUI()
    end
    
    forward()
    
    return snapshot.create(script.name, {}, backward, forward)
end
 

return script
