# RobloxSharp
C# API for performing automated tasks on ROBLOX.com

<b><h1>Features</h1></b>
<ul>
<li>Login to get authentication cookies / tokens</li>
<li>Check messages and automate responses</li>
<li>Monitor trades and build trade responses / requests</li>
</ul>

<b><h2>Coming soon</h2></b>
<ul>
<li>Purchase/sell assets or limiteds</li>
<li>Automatic forum posting</li>
<li>Automatic commenting</li>
<li>Friend request support</li>
</ul>

<b><h1>Examples</h1></b>

<b><h2>Logging in</h2></b>
```
CookieContainer collection;
RobloxLogin login = new RobloxLogin("HomeguardDev", "hunter2", out collection);
String authCookies = login.authCookies;
```
<b>The rest of the examples here will assume that you have already logged in and have authCookies stored</b>

<b><h2>Getting all inbound trades</h2></b>
```
RobloxTradeHandler tradeHandler = new RobloxTradeHandler();
String XRSFToken = RobloxUtils.getXSRFToken(login.authCookies);
TradeList list = tradeHandler.fetchTrades(login.authCookies, XRSFToken, TradeType.Inbound, 0);
foreach (TradeSession tradeSession in list.Data)
{
  TradeDetailsData info = tradeHandler.getTradeInfo(tradeSession.TradeSessionID, XRSFToken, login.authCookies).data;
  Debug.WriteLine(RobloxUtils.getUsername(tradeSession.TradePartnerID).Username + " has an inbound trade with you!");
  TradeObject trade = RobloxUtils.makeTradeObject(info);
  Debug.WriteLine("They are offering:");
  foreach (InventoryItem item in trade.receiving.OfferList)
  {
    Debug.WriteLine("\t" + item.Name);
  }
  Debug.WriteLine("For your: ");
  foreach (InventoryItem item in trade.sending.OfferList)
  {
    Debug.WriteLine("\t" + item.Name);
  }
}
```
<b><h2>Accepting all trades above a certain RAP</h2></b>
```
int myId = RobloxUtils.getUserId("HomeguardDev").Id;
foreach (TradeSession tradeSession in list.Data)
{
  TradeDetailsData info = tradeHandler.getTradeInfo(tradeSession.TradeSessionID, XRSFToken, login.authCookies).data;
  Debug.WriteLine(RobloxUtils.getUsername(tradeSession.TradePartnerID).Username + " has an inbound trade with you!");
  TradeObject trade = RobloxUtils.makeTradeObject(info);
  if (trade.receiving.OfferValue > trade.sending.OfferValue) //accepts all trades where the receiving RAP is higher then your RAP
  {
    String json = tradeHandler.createTradeRequest(myId + "", tradeSession.TradePartnerID, trade);
    tradeHandler.sendTrade(TradeResponseType.Accept, json, XRSFToken, tradeSession.TradeSessionID, login.authCookies);
  }
}
```
<b><h2>Reading all inbound messages</h2></b>
```
RobloxMessageHandler messageHandler = new RobloxMessageHandler();
String XRSFToken = RobloxUtils.getXSRFToken(login.authCookies);
MessageCollection messages = messageHandler.getNewMessages("0", login.authCookies);
foreach (Message message in messages.Collection)
{
  Debug.WriteLine(message.Sender.UserName + "->" + message.Recipient.UserName + "\n" + message.Body);
}
```
<b><h2>Sending a message</h2></b>
```
RobloxMessageHandler messageHandler = new RobloxMessageHandler();
messageHandler.sendMessage(7904, "Hey Me!", "What's up?", login.authCookies);
```
<b><h2>Purchasing a limited item</h2></b>
```
RobloxPurchaseHandler purchase = new RobloxPurchaseHandler();
bool response = purchase.requestLimitedPurchase(1337, 123, 1000, login.authCookies); //1337 is assetID, 123 is userAssetOptionId, and 1000 is the price
Debug.WriteLine(response);
```
