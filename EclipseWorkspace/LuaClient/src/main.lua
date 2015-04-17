require "Cocos2d"
require "Cocos2dConstants"
require "NetworkConstants"

-- cclog
cclog = function(...)
    print(string.format(...))
end


--for CCLuaEngine traceback
function __G__TRACKBACK__(msg)
    cclog("----------------------------------------")
    cclog("LUA ERROR: " .. tostring(msg) .. "\n")
    cclog(debug.traceback())
    cclog("----------------------------------------")
    return msg
end

local function main()

    local size = cc.Director:getInstance():getWinSize()


    collectgarbage("collect")
    -- avoid memory leak
    collectgarbage("setpause", 100)
    collectgarbage("setstepmul", 5000)

    -- initialize director
    local director = cc.Director:getInstance()
    local glview = director:getOpenGLView()
    if nil == glview then
        glview = cc.GLViewImpl:createWithRect("HelloLua", cc.rect(0,0,900,640))
        director:setOpenGLView(glview)
    end

    glview:setDesignResolutionSize(480, 320, cc.ResolutionPolicy.NO_BORDER)


    --turn on display FPS
    director:setDisplayStats(true)

    --set FPS. the default value is 1.0/60 if you don't call this
    director:setAnimationInterval(1.0 / 60)

	cc.FileUtils:getInstance():addSearchPath("src")
	cc.FileUtils:getInstance():addSearchPath("res")
	local schedulerID = 0
    --support debug
    local targetPlatform = cc.Application:getInstance():getTargetPlatform()
    if (cc.PLATFORM_OS_IPHONE == targetPlatform) or (cc.PLATFORM_OS_IPAD == targetPlatform) or 
       (cc.PLATFORM_OS_ANDROID == targetPlatform) or (cc.PLATFORM_OS_WINDOWS == targetPlatform) or
       (cc.PLATFORM_OS_MAC == targetPlatform) then
        cclog("result is ")
		--require('debugger')()
        
    end
--    require "hello2"
--    cclog("result is " .. myadd(1, 1))

--wsSendText  = cc.WebSocket:create("ws://127.0.0.1:8081")


    local visibleSize = cc.Director:getInstance():getVisibleSize()
    local origin = cc.Director:getInstance():getVisibleOrigin()

--    -- create farm
--    local function createLayerFarm()
--        local layerFarm = cc.Layer:create()
--
--        -- add in farm background
--        local bg = cc.Sprite:create("farm.jpg")
--        bg:setPosition(origin.x + visibleSize.width / 2 + 80, origin.y + visibleSize.height / 2)
--        layerFarm:addChild(bg)
--
--        -- add land sprite
--        for i = 0, 3 do
--            for j = 0, 1 do
--                local spriteLand = cc.Sprite:create("land.png")
--                spriteLand:setPosition(200 + j * 180 - i % 2 * 90, 10 + i * 95 / 2)
--                layerFarm:addChild(spriteLand)
--            end
--        end
--
--        -- add crop
--        local frameCrop = cc.SpriteFrame:create("crop.png", cc.rect(0, 0, 105, 95))
--        for i = 0, 3 do
--            for j = 0, 1 do
--                local spriteCrop = cc.Sprite:createWithSpriteFrame(frameCrop);
--                spriteCrop:setPosition(10 + 200 + j * 180 - i % 2 * 90, 30 + 10 + i * 95 / 2)
--                layerFarm:addChild(spriteCrop)
--            end
--        end
--

--
--        local function onNodeEvent(event)
--           if "exit" == event then
--               cc.Director:getInstance():getScheduler():unscheduleScriptEntry(schedulerID)
--           end
--        end
--        layerFarm:registerScriptHandler(onNodeEvent)
--
--        return layerFarm
--    end

    -- create menu
    local function createLayerMenu()
        local layerMenu = cc.Layer:create()

        local menuPopup, menuTools, effectID

        local function menuCallbackClosePopup()
            -- stop test sound effect
            cc.SimpleAudioEngine:getInstance():stopEffect(effectID)
            menuPopup:setVisible(false)
        end

        local function menuCallbackOpenPopup()
            -- loop test sound effect
            local effectPath = cc.FileUtils:getInstance():fullPathForFilename("effect1.wav")
            effectID = cc.SimpleAudioEngine:getInstance():playEffect(effectPath)
            menuPopup:setVisible(true)
        end

        -- add a popup menu
        local menuPopupItem = cc.MenuItemImage:create("menu2.png", "menu2.png")
        menuPopupItem:setPosition(0, 0)
        menuPopupItem:registerScriptTapHandler(menuCallbackClosePopup)
        menuPopup = cc.Menu:create(menuPopupItem)
        menuPopup:setPosition(origin.x + visibleSize.width / 2, origin.y + visibleSize.height / 2)
        menuPopup:setVisible(false)
        layerMenu:addChild(menuPopup)
        
        -- add the left-bottom "tools" menu to invoke menuPopup
        local menuToolsItem = cc.MenuItemImage:create("menu1.png", "menu1.png")
        menuToolsItem:setPosition(0, 0)
        menuToolsItem:registerScriptTapHandler(menuCallbackOpenPopup)
        menuTools = cc.Menu:create(menuToolsItem)
        local itemWidth = menuToolsItem:getContentSize().width
        local itemHeight = menuToolsItem:getContentSize().height
        menuTools:setPosition(origin.x + itemWidth/2, origin.y + itemHeight/2)
        layerMenu:addChild(menuTools)

        return layerMenu
    end

    -- play background music, preload effect
    local bgMusicPath = cc.FileUtils:getInstance():fullPathForFilename("background.mp3") 
--    cc.SimpleAudioEngine:getInstance():playMusic(bgMusicPath, true)
    local effectPath = cc.FileUtils:getInstance():fullPathForFilename("effect1.wav")
    cc.SimpleAudioEngine:getInstance():preloadEffect(effectPath)

    -- run
    local sceneGame = cc.Scene:create()

    --sceneGame:addChild(createLayerFarm())
    --sceneGame:addChild(createLayerMenu())
    --require "websocketlayer"
    --sceneGame:addChild(createLayerWebSocket())
    require "TileMaps"
    require "Player"
    local tileLayer = TileMapEditTest()
    sceneGame:addChild(tileLayer)
	
    -- add Player
    local sprite = createPlayer()
    tileLayer:addChild(sprite)
    
	require "comms"
    CreateCommms()
	
	if cc.Director:getInstance():getRunningScene() then
		cc.Director:getInstance():replaceScene(sceneGame)
	else
		cc.Director:getInstance():runWithScene(sceneGame)
	end
end


local status, msg = xpcall(main, __G__TRACKBACK__)
if not status then
    error(msg)
end


