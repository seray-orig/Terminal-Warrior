-- Стандарт имён функций - имена абстрактных классов
-- Сцена должна строиться на основе этих трёх функций

function InputHandler()

end

function EngineUpdater()

end

function FrameRenderer()
	frameClear()
	cursorPos(0, 0)
	write('Это UTF-8 Ура!')
end