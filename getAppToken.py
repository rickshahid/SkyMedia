import sys
import jwt
import time

appId = sys.argv[0]
appKey = sys.argv[1]
appTokenSeconds = sys.argv[2]
appTokenEncryption = sys.argv[3]

timeEpochSeconds = int(time.time())
appTokenPayload = {
  'iss': appId,
  'iat': timeEpochSeconds,
  'exp': timeEpochSeconds + int(appTokenSeconds)
}

appToken = jwt.encode(appTokenPayload, appKey, algorithm=appTokenEncryption)
print(appToken.decode())
