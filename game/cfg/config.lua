-- Конфигурация игры
-- CreateConVar(имя, значение, (для числа) допустимый минимум, максимум)

-- Иммутабельно. Загружается при инициализации программы
CreateConVar('game_title', 'Terminal Warrior')

-- Мутабельно. Можно менять на ходу
CreateConVar('second_scene_name', 'cmd')
CreateConVar('second_scene_char', '~')
CreateConVar('hot_lua_reload_char', '}')

CreateConVar('fps_target', 20, 1)
CreateConVar('DebugChar', ' ')