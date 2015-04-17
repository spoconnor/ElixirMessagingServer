require "Cocos2d"
require "Cocos2dConstants"
require "NetworkConstants"

-- cclog
--cclog = function(...)
--    print(string.format(...))
--end

local wsSendBinary = nil
local receiveBinaryTimes = 0
local isOpen = false

function IsOpen()
    return isOpen
end
    
function CreateCommms()
    cclog("[Commms] CreateCommms")
    local serverurl = "ws://echo.websocket.org" 
    --local serverurl = "ws://10.1.1.4:8081"

    cclog("[Commms] Create WebSocket...")
    wsSendBinary = cc.WebSocket:create(serverurl)
    cclog("[Commms] WebSocket Created")

    local function wsConnectionOpen(strData)
        cclog("[Commms] Connection open.")
        isOpen = true
        RegisterClient()
    end

    local function wsReceivedMessage(strData)
        cclog("[Commms] Received message."..strData)
        receiveBinaryTimes = receiveBinaryTimes + 1
    end

    local function wsConnectionClose(strData)
        cclog("[Commms] Connection instance closed.")
        wsSendBinary = nil
    end

    local function wsConnectionError(strData)
        cclog("[Commms] Connection Error was fired")
    end

    if nil ~= wsSendBinary then
        wsSendBinary:registerScriptHandler(wsConnectionOpen,cc.WEBSOCKET_OPEN)
        wsSendBinary:registerScriptHandler(wsReceivedMessage,cc.WEBSOCKET_MESSAGE)
        wsSendBinary:registerScriptHandler(wsConnectionClose,cc.WEBSOCKET_CLOSE)
        wsSendBinary:registerScriptHandler(wsConnectionError,cc.WEBSOCKET_ERROR)
    end
end

function Close()
    cclog("[Commms] Close")
    if nil ~= wsSendBinary then
        wsSendBinary:close()
    end
end

--Send Binary
function RegisterClient()
    cclog("[Commms] RegisterClient")
    if nil ~= wsSendBinary then
        if cc.WEBSOCKET_STATE_OPEN == wsSendBinary:getReadyState() then
            cclog("Send Binary WS is waiting...")
            wsSendBinary:sendString("1|Sean|Hi")
        else
            cclog("send binary websocket instance wasn't ready...")
        end
    end
end

