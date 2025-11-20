x = (x or 1) + 1

local function write(...)
	local content = {}
	for _, value in ipairs({...}) do
    	table.insert(content, value)
  	end
	converter(content)
end

function FrameRenderer()
	frameClear()
	cursorPos(15, 10)
	write('g =', ' ', 10)
end

