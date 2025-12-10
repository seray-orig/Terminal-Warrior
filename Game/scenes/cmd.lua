line = line or {}
linesStory = linesStory or {}

function InputHandler(_readKey, _readChar)
    if (_readKey == 'Enter') then
        local command = {}
        for word in string.gmatch(table.concat(line), "%a+") do
            table.insert(command, word)
        end
        table.insert(linesStory, 1, line)
        if (#linesStory > 7) then
            for i = 8, #linesStory do linesStory[i] = nil end
        end
        line = {}
        CallCommand(command)
    elseif (_readKey == 'Backspace') then
        table.remove(line, #line)
    else
        table.insert(line, _readChar)
    end
end

function FrameRenderer()
    SetCursorPos(2, 1)
    Writeln('Консоль')
    for i = 1, ScrW() do
        Write('_')
    end

    SetCursorPos(1, 4)
    for _,v in ipairs(line) do
        Write(v)
    end

    for line, charTable in ipairs(linesStory) do
        SetCursorPos(1, 5 + line)
        if (line + 6 >= ScrH()) then return end
        for _,v in ipairs(charTable) do
            Write(v)
        end
    end
end

function CallCommand(command)
    local commands = {}
    commands.setscene = function(arg)
        SetScene(arg)
    end
    commands.doscript = function(arg)
        DoScript(arg)
    end

    commands[command[1]](command[2])
end