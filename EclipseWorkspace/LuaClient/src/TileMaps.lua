require "Cocos2d"
require "Cocos2dConstants"

local kTagTileMap = 1
local s_TilesPng = "tiles.png"
local s_LevelMapTga = "levelmap.tga"
local scheduler = cc.Director:getInstance():getScheduler()

function TileMapEditTest()
    cclog("TileMapEditTest")
    local layerTiles = cc.Layer:create()
    
    -- handing touch events
    local function onTouchesMoved(touches, event )
        local diff = touches[1]:getDelta()
        local node = layerTiles:getChildByTag(kTagTileMap)
        local currentPosX, currentPosY= node:getPosition()
        node:setPosition(cc.p(currentPosX + diff.x, currentPosY + diff.y))
    end

    --        local touchBeginPoint = nil
    --        local function onTouchBegan(touch, event)
    --            local location = touch:getLocation()
    --            cclog("onTouchBegan: %0.2f, %0.2f", location.x, location.y)
    --            touchBeginPoint = {x = location.x, y = location.y}
    --            spriteDog.isPaused = true
    --            -- CCTOUCHBEGAN event must return true
    --            return true
    --        end

    --        local function onTouchMoved(touch, event)
    --            local location = touch:getLocation()
    --            cclog("onTouchMoved: %0.2f, %0.2f", location.x, location.y)
    --            if touchBeginPoint then
    --                local cx, cy = layerTiles:getPosition()
    --                layerTiles:setPosition(cx + location.x - touchBeginPoint.x,
    --                    cy + location.y - touchBeginPoint.y)
    --                touchBeginPoint = {x = location.x, y = location.y}
    --            end
    --        end

    --        local function onTouchEnded(touch, event)
    --            local location = touch:getLocation()
    --            cclog("onTouchEnded: %0.2f, %0.2f", location.x, location.y)
    --            touchBeginPoint = nil
    --            spriteDog.isPaused = false
    --        end

    --        local listener = cc.EventListenerTouchOneByOne:create()
    --        listener:registerScriptHandler(onTouchBegan,cc.Handler.EVENT_TOUCH_BEGAN )
    --        listener:registerScriptHandler(onTouchMoved,cc.Handler.EVENT_TOUCH_MOVED )
    --        listener:registerScriptHandler(onTouchEnded,cc.Handler.EVENT_TOUCH_ENDED )
    --        local eventDispatcher = layerTiles:getEventDispatcher()
    --        eventDispatcher:addEventListenerWithSceneGraphPriority(listener, layerTiles)

    cclog("TileMapEditTest events")
    local listener = cc.EventListenerTouchAllAtOnce:create()
    listener:registerScriptHandler(onTouchesMoved,cc.Handler.EVENT_TOUCHES_MOVED )
    local eventDispatcher = layerTiles:getEventDispatcher()
    eventDispatcher:addEventListenerWithSceneGraphPriority(listener, layerTiles)

    cclog("TileMapEditTest tiles")
    local  map = cc.TileMapAtlas:create(s_TilesPng, s_LevelMapTga, 32, 32)
    -- Create an Aliased Atlas
    map:getTexture():setAliasTexParameters()

    local  s = map:getContentSize()
    cclog("ContentSize: %f, %f", s.width,s.height)

    -- If you are not going to use the Map, you can free it now
    -- [tilemap releaseMap)
    -- And if you are going to use, it you can access the data with:
    local function updateMap(dt)
        -- IMPORTANT
        --   The only limitation is that you cannot change an empty, or assign an empty tile to a tile
        --   The value 0 not rendered so don't assign or change a tile with value 0

        local  tilemap = layerTiles:getChildByTag(kTagTileMap)

        --
        -- For example you can iterate over all the tiles
        -- using this code, but try to avoid the iteration
        -- over all your tiles in every frame. It's very expensive
        --    for(int x=0 x < tilemap.tgaInfo:width x++)
        --        for(int y=0 y < tilemap.tgaInfo:height y++)
        --            Color3B c =[tilemap getTileAt:local Make(x,y))
        --            if( c.r != 0 )
        --                --------cclog("%d,%d = %d", x,y,c.r)
        --            end
        --        end
        --    end

        -- NEW since v0.7
        local c = tilemap:getTileAt(cc.p(13,21))
        c.r = c.r + 1
        c.r = c.r % 50

        if( c.r==0) then
            c.r=1
        end
        -- NEW since v0.7
        tilemap:setTile(c, cc.p(13,21) )
    end

    local schedulerEntry = nil
    local function onNodeEvent(event)
        if event == "enter" then
            schedulerEntry = scheduler:scheduleScriptFunc(updateMap, 0.2, false)
        elseif event == "exit" then
            scheduler:unscheduleScriptEntry(schedulerEntry)
        end
    end

    layerTiles:registerScriptHandler(onNodeEvent)

    layerTiles:addChild(map, 0, kTagTileMap)

    map:setAnchorPoint( cc.p(0, 0) )
    map:setPosition( cc.p(-20,-200) )


    --        map:setAnchorPoint( cc.p(0, 0.5) )
    --        local scale = cc.ScaleBy:create(4, 0.8)
    --        local scaleBack = scale:reverse()
    --        local  seq = cc.Sequence:create(scale, scaleBack)
    --        map:runAction(cc.RepeatForever:create(seq))

    cclog("TileMapEditTest done")
    return layerTiles
end
