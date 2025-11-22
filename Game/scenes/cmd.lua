line = line or {}
linesStory = linesStory or {}

function InputHandler()
    if (_readKey == 'Enter') then
        table.insert(linesStory, 1, line)
        line = {}
    end
    table.insert(line, _readChar)
end

function FrameRenderer()
    SetCursorPos(2, 1)
    println('Консоль')
    for i = 1, ScrW() do
        print('_')
    end

    SetCursorPos(1, 4)
    for _,v in ipairs(line) do
        print(v)
    end

    for line, charTable in ipairs(linesStory) do
        SetCursorPos(1, 5 + line)
        if (line + 6 >= ScrH()) then return end
        for _,v in ipairs(charTable) do
            print(v)
        end
    end
end