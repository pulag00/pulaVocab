import json
import urllib.request
import urllib.error

url = 'http://localhost:5000/api/vocabulary/autocomplete'
body = json.dumps({
    'Term': 'word',
    'Language': 0,
    'TranslationLanguage': 'Spanish'
}).encode('utf-8')
req = urllib.request.Request(url, data=body, headers={
                             'Content-Type': 'application/json'})
try:
    with urllib.request.urlopen(req) as r:
        print('SUCCESS')
        print(r.read().decode('utf-8'))
except urllib.error.HTTPError as e:
    print('ERROR', e.code)
    print(e.read().decode('utf-8'))
except Exception as ex:
    print('EXCEPTION', ex)
