NpcTable = {}
EntityTable = {}

ply = SpawnPlayer('Steve', 20, {5,5}, 'i')

include('world.lua')
include('mobs.lua')

GetNpcTable(NpcTable)
GetEntityTable(EntityTable)