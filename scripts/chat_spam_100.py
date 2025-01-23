import requests
import logging
import json
import sys
import time

#from signalrcore.hub_connection import HubConnection
#import signalrcore.hub_connection
#from signalrcore import HubConnection
from signalrcore.hub_connection_builder import HubConnectionBuilder

# consts
targetBroadcaster = 'mariarenard';
CHAT_API_PATH = 'http://188.17.155.106:39000/chat'
API_URL = 'http://188.17.155.106:39000/api/test'
developerJwt = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiZGV2ZW9wZXJfMjY5NV82ZWYiLCJuYmYiOjE3MzY0Mjc0MTAsImV4cCI6MjUxNDAyNzQxMH0.Qj61wBxPqlMWNqRSAPsSBAzh4XSi_ljxpd6QJfo8Auk'

# readonly 
messages = ['Hello', 'How are you?', 'cool stream :P', 'heh', 'VOSTANIE MASHIN', 'go go go', '123', 'send surrikens in chat']
messagesCount = len(messages) 

colors = ['red', 'blue', 'green', 'black', 'yellow']
colorsCount = len(colors) 

def apiCallGet(path, authValue): 
    requestUrl = API_URL + path;
    headers = {'Authorization': 'Bearer ' + authValue }
    
    response = requests.get(requestUrl, headers=headers)
    print('Calling API, url: ' + requestUrl + ', response: ' + str(response))

    statusOk = (response.status_code >= 200 and response.status_code < 400)
    if statusOk:
        responseJson = json.loads( response.text );
        return { 'statusCode': response.status_code, 'json': responseJson, 'success': statusOk and responseJson['success'] == 'true' }
    else:
        return { 'statusCode': response.status_code, 'success': 'false' }
     
def startStartFromUser(user):
    jwt = user['jwt']['token'];
    username = user['username'];

    #my_string[a:b] gives you a substring from index a to (b - 1).
    print('User ' + username + ' ' + jwt[0:4] + ' start spamming.')

    connection = HubConnectionBuilder()\
            .with_url(CHAT_API_PATH, options={ "access_token_factory": lambda: jwt, "headers": { "Authorization": jwt } })\
            .configure_logging(logging.INFO)\
            .build()
            #.with_automatic_reconnect()\
            #.build()
            
    connection.on_error(lambda data: print(f"An exception was thrown closed{data.error}"))
    
    connection.start()
    
    time.sleep(1)
    
    connection.send('JoinChat', [ targetBroadcaster ])
    
    time.sleep(1)
    
    #connection.send('Send', [ 'Hello!', 'red', targetBroadcaster ])
    
    colorIndex = random.randint(0, colorsCount)
    color = colors[colorIndex]
    
    # sending request 1 per second
    while True: 
        
        messageIndex = random.randint(0, messagesCount)
        message = messages[messageIndex];
        
        connection.send('Send', [ message, color, targetBroadcaster ])
    
        print(username + ' sent ' + message)
    
        time.sleep(3)
    
    #for i in range(1)
     #   
        #break;
     
def startSpam(plutoUsers):
    #print('Pluto users:')
    
    for usr in plutoUsers[0:3]:
        #print('\t' + str(usr) )
        startStartFromUser( usr )

    sys.exit();

print('****** CHAT SPAM ******')
print('Developer JWT: ' + developerJwt)

hellojsonRequest = apiCallGet('/hellojson', '')
#print(hellojsonRequest['success'])
if hellojsonRequest['success']:
    plutousersRequest = apiCallGet('/plutousers', developerJwt)

    if plutousersRequest['success']:
        if plutousersRequest['json']['count'] == 0: 
            print('No pluto users found! Fetching new ones...')
                    
            fetchPlutoUsers = apiCallGet('/seedplutousers', developerJwt)
            
            if fetchPlutoUsers['success']:
                startSpam(fetchPlutoUsers['json']['users']);
                sys.exit()
        else:
           startSpam(plutousersRequest['json']['users']);
    
print('Status code: 500. Server is shutdown.');
sys.exit()


    


