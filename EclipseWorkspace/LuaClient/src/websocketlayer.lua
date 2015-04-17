require "Cocos2d"
require "Cocos2dConstants"
require "NetworkConstants"

function createLayerWebSocket()

    -- cclog
    cclog = function(...)
        print(string.format(...))
    end

    cclog("WebSocketTestLayer")
    --local serverurl = "ws://echo.websocket.org" 
    local serverurl = "ws://10.1.1.4:8081"
    local layer   = cc.Layer:create()
    local winSize = cc.Director:getInstance():getWinSize()
    local MARGIN = 40
    local SPACE  = 35

    local s_fontPath = "fonts/Marker Felt.ttf"
    local wsSendBinary = nil
    local sendBinaryStatus = nil
    local errorStatus  = nil
    local receiveBinaryTimes = 0
    
    local label = cc.Label:createWithTTF("WebSocket Test", s_fontPath, 28)
    label:setAnchorPoint(cc.p(0.5, 0.5))
    label:setPosition(cc.p( winSize.width / 2, winSize.height - MARGIN))
    layer:addChild(label, 0)

    local menuRequest = cc.Menu:create()
    menuRequest:setPosition(cc.p(0, 0))
    layer:addChild(menuRequest)

    --Send Binary
    local function onMenuSendBinaryClicked()
    	cclog("WebSocketTestLayer onMenuSendBinaryClicked")
        if nil ~= wsSendBinary then
            if cc.WEBSOCKET_STATE_OPEN == wsSendBinary:getReadyState() then
               sendBinaryStatus:setString("Send Binary WS is waiting...")
               wsSendBinary:sendString("1|Sean|Hi")
            else
                local warningStr = "send binary websocket instance wasn't ready..."
                sendBinaryStatus:setString(warningStr)
            end
        end
    end

    local labelSendBinary = cc.Label:createWithTTF("Send Binary", s_fontPath, 22)
    labelSendBinary:setAnchorPoint(cc.p(0.5, 0.5))
    local itemSendBinary = cc.MenuItemLabel:create(labelSendBinary)
    itemSendBinary:registerScriptTapHandler(onMenuSendBinaryClicked)
    itemSendBinary:setPosition(cc.p(winSize.width / 2, winSize.height - MARGIN - 2 * SPACE))
    menuRequest:addChild(itemSendBinary)

    --Send Binary Status Label
    sendBinaryStatus = cc.Label:createWithTTF("Send Binary WS is waiting...", s_fontPath, 14, cc.size(160, 100), cc.VERTICAL_TEXT_ALIGNMENT_CENTER, cc.VERTICAL_TEXT_ALIGNMENT_TOP)
    sendBinaryStatus:setAnchorPoint(cc.p(0, 0))
    sendBinaryStatus:setPosition(cc.p(160, 25))
    layer:addChild(sendBinaryStatus)

    cclog("Create WebSocket...")
    wsSendBinary = cc.WebSocket:create(serverurl)
    cclog("WebSocket Created")

    local function wsSendBinaryOpen(strData)
        cclog("SendBinary open.")
        sendBinaryStatus:setString("Send Binary WS was opened.")
    end

    local function wsSendBinaryMessage(strData)
        cclog("SendBinary received message.")
        receiveBinaryTimes = receiveBinaryTimes + 1
        local strInfo = "Received:"..strData
        sendBinaryStatus:setString(strInfo)
    end

    local function wsSendBinaryClose(strData)
        cclog("SendBinary instance closed.")
        sendBinaryStatus = nil
        wsSendBinary = nil
    end

    local function wsSendBinaryError(strData)
        cclog("sendBinary Error was fired")
    end

    if nil ~= wsSendBinary then
        wsSendBinary:registerScriptHandler(wsSendBinaryOpen,cc.WEBSOCKET_OPEN)
        wsSendBinary:registerScriptHandler(wsSendBinaryMessage,cc.WEBSOCKET_MESSAGE)
        wsSendBinary:registerScriptHandler(wsSendBinaryClose,cc.WEBSOCKET_CLOSE)
        wsSendBinary:registerScriptHandler(wsSendBinaryError,cc.WEBSOCKET_ERROR)
    end

    local function OnExit(strEventName)
        cclog("OnExit event:"..strEventName)
        if "exit" == strEventName then
            if nil ~= wsSendBinary then
                wsSendBinary:close()
            end
        end
    end

    layer:registerScriptHandler(OnExit)

    return layer
end

