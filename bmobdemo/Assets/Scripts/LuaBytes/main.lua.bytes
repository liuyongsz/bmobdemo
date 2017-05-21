require "common/define"

LuaEvent = require 'common/events';

--require "rapidjson"
--local rapidjson = require('rapidjson')
--local t = rapidjson.decode('{"a":123}')
--print(t.a)
--t.a = 456
--local s = rapidjson.encode(t)
--print('json', s)

function mainStart()
	requireAllPanels()
end

function requireAllPanels()
  for i = 1, #PanelNames do
		require (LuaAssetName.lua.."panels/"..tostring(PanelNames[i]))
		print(PanelNames[i])

	end
end
