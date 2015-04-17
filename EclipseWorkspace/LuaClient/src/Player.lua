
local visibleSize = cc.Director:getInstance():getVisibleSize()
local origin = cc.Director:getInstance():getVisibleOrigin()

function createPlayer()
    local frameWidth = 105
    local frameHeight = 95

    -- create dog animate
    local textureDog = cc.Director:getInstance():getTextureCache():addImage("dog.png")
    local rect = cc.rect(0, 0, frameWidth, frameHeight)
    local frame0 = cc.SpriteFrame:createWithTexture(textureDog, rect)
    rect = cc.rect(frameWidth, 0, frameWidth, frameHeight)
    local frame1 = cc.SpriteFrame:createWithTexture(textureDog, rect)

    local sprite = cc.Sprite:createWithSpriteFrame(frame0)
    sprite.isPaused = false
    sprite:setPosition(origin.x, origin.y + visibleSize.height / 4 * 3)
--[[
    local animFrames = CCArray:create()

    animFrames:addObject(frame0)
    animFrames:addObject(frame1)
]]--

    local animation = cc.Animation:createWithSpriteFrames({frame0,frame1}, 0.5)
    local animate = cc.Animate:create(animation);
    sprite:runAction(cc.RepeatForever:create(animate))

    -- moving at every frame
    local function tick()
    if sprite.isPaused then return end
    local x, y = sprite:getPosition()
        if x > origin.x + visibleSize.width then
            x = origin.x
        else
            x = x + 1
        end

    sprite:setPositionX(x)
    end

    schedulerID = cc.Director:getInstance():getScheduler():scheduleScriptFunc(tick, 0, false)

    return sprite
end
    