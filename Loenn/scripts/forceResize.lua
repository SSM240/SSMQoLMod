local roomStruct = require("structs.room")

local script = {
    name = "forceResize",
    displayName = "Force Resize",
    tooltip = "Resizes a room, ignoring the 40x23 minimum size",
    parameters = {
        width = "",
        height = ""
    },
    fieldOrder = {
        "width",
        "height"
    },
    fieldInformation = {
        width = {
            fieldType = "integer",
            allowEmpty = true
        },
        height = {
            fieldType = "integer",
            allowEmpty = true
        }
    }
}

function script.run(room, args)
    if (args.width and args.width ~= "") then
        local width = tonumber(args.width)
        roomStruct.directionalResize(room, "right", width - room.width / 8)
    end
    if (args.height and args.height ~= "") then
        local height = tonumber(args.height)
        roomStruct.directionalResize(room, "down", height - room.height / 8)
    end
end

return script
