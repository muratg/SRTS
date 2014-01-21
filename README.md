# SRTS

SignalR-TypeScript declarations generator

## WHAT

This tool generates TypeScript declarations files for your SignalR hubs

## WHY

You might want to write your SignalR client code in TypeScript instead of JavaScript for obvious reasons. Some of the benefits are:

* Dev/build time type checking. This is especially helpful when the churn in your code is high
* Better tooling support. While intellisense for JavaScript is getting better, it's not on par with TypeScript

## HOW

### Using NuGet

* Install SRTS NuGet package
* types.tt and signalr.d.ts files are generated in your project. You can move them / rename them if you want
* Definition of your types will be generated in types.d.ts file.  You can reference this file from your .ts files
* A basic piece "starter" code is provided on top of types.d.ts file.  You can copy paste this code to your .ts file

### Using command line tool

* srts.exe installed in project packages folder (installed with the srts NuGet package,) and can be taken from there
* Copy srts.exe to the folder where your assembly is in 
* Run "srts.exe ASSEMBLYNAME.DLL > myTypes.d.ts"

## FEATURES

In addition to supporting all basic types, SRTS also supports:

* Tuple
* Nullable types
* Task results
* List and Array like types (IEnumerable, etc...)
* Dictionary like types
* Enums
* Custom classes/interfaces

### SAMPLE OUTPUT 

## Sample output from StockTicker

	/* Sample code -- create a .ts file, and paste the following to get started

	/// <reference path="types.d.ts" />
	/// <reference path="signalr.d.ts" />

	//  ---- Change the name name of d.ts file above based on your .tt file ---- 
	var stockTicker: IStockTickerHubProxy = (<any>$.connection).stockTicker;
	$.connection.hub.start().done((a) => {
		alert('connected.'); // .... 
	});

	*/

	// Client interfaces
	// These are expected to be defined by the user elsewhere
	// (This file is auto-generated)
	interface IStockTickerHubClient { /* To be defined elsewhere... */ }

	// Promise interface
	interface IPromise<T> {
		done(cb: (result: T) => any): IPromise<T>;
		error(cb: (error: any) => any): IPromise<T>;
	}

	// Data interfaces 
	interface Stock {
		Symbol: string;
		DayOpen: number;
		DayLow: number;
		DayHigh: number;
		LastChange: number;
		Change: number;
		PercentChange: number;
		Price: number;
	}

	// Hub interfaces 
	interface IStockTickerHub {
		getAllStocks(): IPromise<Array<Stock>>;
		getMarketState(): IPromise<string>;
		openMarket(): IPromise<void>;
		closeMarket(): IPromise<void>;
		reset(): IPromise<void>;
	}

	// Generetated proxies 
	interface IStockTickerHubProxy {
		 server: IStockTickerHub;
		 client: IStockTickerHubClient;
	}

## Sample output from JabbR

	// Client interfaces
	// These are expected to be defined by the user elsewhere
	// (This file is auto-generated)
	interface IMonitorClient { /* To be defined elsewhere... */ }
	interface IChatClient { /* To be defined elsewhere... */ }

	// Promise interface
	interface IPromise<T> {
		done(cb: (result: T) => any): IPromise<T>;
		error(cb: (error: any) => any): IPromise<T>;
	}

	// Data interfaces 
	interface ClientMessage {
		Id: string;
		Content: string;
		Room: string;
	}
	interface UserViewModel {
		Name: string;
		Hash: string;
		Active: boolean;
		Status: string;
		Note: string;
		AfkNote: string;
		IsAfk: boolean;
		Flag: string;
		Country: string;
		LastActivity: string;
		IsAdmin: boolean;
	}
	interface LobbyRoomViewModel {
		Name: string;
		Count: number;
		Private: boolean;
		Closed: boolean;
		Topic: string;
	}
	interface MessageViewModel {
		HtmlEncoded: boolean;
		Id: string;
		Content: string;
		HtmlContent: string;
		When: string;
		User: UserViewModel;
		MessageType: number;
		ImageUrl: string;
		Source: string;
	}
	interface RoomViewModel {
		Name: string;
		Count: number;
		Private: boolean;
		Topic: string;
		Closed: boolean;
		Welcome: string;
		Users: Array<UserViewModel>;
		Owners: Array<string>;
		RecentMessages: Array<MessageViewModel>;
	}
	interface ClientNotification {
		Room: string;
		ImageUrl: string;
		Source: string;
		Content: string;
	}

	// Hub interfaces 
	interface IMonitor {
	}
	interface IChat {
		onConnected(): IPromise<void>;
		join(): IPromise<void>;
		join(reconnecting: boolean): IPromise<void>;
		send(content: string, roomName: string): IPromise<boolean>;
		send(clientMessage: ClientMessage): IPromise<boolean>;
		getUserInfo(): IPromise<UserViewModel>;
		onReconnected(): IPromise<void>;
		onDisconnected(): IPromise<void>;
		getCommands(): IPromise<any>;
		getShortcuts(): IPromise<any>;
		getRooms(): IPromise<Array<LobbyRoomViewModel>>;
		getPreviousMessages(messageId: string): IPromise<Array<MessageViewModel>>;
		getRoomInfo(roomName: string): IPromise<RoomViewModel>;
		postNotification(notification: ClientNotification): IPromise<void>;
		postNotification(notification: ClientNotification, executeContentProviders: boolean): IPromise<void>;
		typing(roomName: string): IPromise<void>;
		updateActivity(): IPromise<void>;
	}

	// Generetated proxies 
	interface IMonitorProxy {
		 server: IMonitor;
		 client: IMonitorClient;
	}
	interface IChatProxy {
		 server: IChat;
		 client: IChatClient;
	}

## Sample output from ShootR

	/* Sample code -- create a .ts file, and paste the following to get started

	/// <reference path="types.d.ts" />
	/// <reference path="signalr.d.ts" />

	//  ---- Change the name name of d.ts file above based on your .tt file ---- 
	var h: IGameHubProxy = (<any>$.connection).h;
	$.connection.hub.start().done((a) => {
		alert('connected.'); // .... 
	});

	*/

	// Client interfaces
	// These are expected to be defined by the user elsewhere
	// (This file is auto-generated)
	interface IGameHubClient { /* To be defined elsewhere... */ }

	// Promise interface
	interface IPromise<T> {
		done(cb: (result: T) => any): IPromise<T>;
		error(cb: (error: any) => any): IPromise<T>;
	}

	// Data interfaces 

	// Hub interfaces 
	interface IGameHub {
		onConnected(): IPromise<void>;
		onReconnected(): IPromise<void>;
		onDisconnected(): IPromise<void>;
		ping(): IPromise<string>;
		fire(): IPromise<number>;
		startFire(): IPromise<void>;
		stopFire(): IPromise<number>;
		initializeClient(registrationID: string): IPromise<any>;
		initializeController(registrationID: string): IPromise<any>;
		readyForPayloads(): IPromise<void>;
		resetMovement(movementList: Array<string>, pingBack: boolean, commandID: number): IPromise<void>;
		startAndStopMovement(toStop: string, toStart: string, pingBack: boolean, commandID: number): IPromise<void>;
		registerMoveStart(movement: string, pingBack: boolean, commandID: number): IPromise<void>;
		registerMoveStop(movement: string, pingBack: boolean, commandID: number): IPromise<void>;
		registerAbilityStart(abilityName: string, pingBack: boolean, commandID: number): IPromise<void>;
		registerAbilityStop(abilityName: string, pingBack: boolean, commandID: number): IPromise<void>;
		registerAbilityStop(ability: string): IPromise<void>;
		changeViewport(viewportWidth: number, viewportHeight: number): IPromise<void>;
		readyForLeaderboardPayloads(): IPromise<void>;
		stopLeaderboardPayloads(): IPromise<void>;
	}

	// Generetated proxies 
	interface IGameHubProxy {
		 server: IGameHub;
		 client: IGameHubClient;
	}
