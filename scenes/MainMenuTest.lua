-- Стандарт имён функций - имена абстрактных классов
-- Сцена должна строиться на основе этих трёх функций

Text = Text or { 1, {
    "Новая игра",
    "Выход"
}}

function InputHandler()
    if (_readKey == 'DownArrow') then
        Text[1] = math.min(Text[1] + 1, #Text[2])
    elseif (_readKey == 'UpArrow') then
        Text[1] = math.max(Text[1] - 1, 1)
    elseif (_readKey == 'Enter') then
        if (Text[1] == 1) then
            SetScene('Gameplay')
        elseif (Text[1] == 2) then
            _ShutDownGame()
        end
    end
end

function EngineUpdater()

end

function FrameRenderer()
    SetCursorPos(ScrW()/2, ScrH()/2 - #Text)
    for num, line in ipairs(Text[2]) do
        local len = utf8.len(line)
        SetCursorPos(CurL() - len/2, CurT())
        Write(line)
        if (num == Text[1]) then
            SetCursorPos(CurL() - 2 - len, CurT())
            Write('>')
            SetCursorPos(CurL() + 2 + len, CurT())
            Write('<')
        end
        SetCursorPos(ScrW()/2, CurT() + 2)
    end
end