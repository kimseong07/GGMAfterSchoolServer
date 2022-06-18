//설치하지 않아도 쓸 수 있는 기본 내장 모듈들이 존재해
const http = require('http');  //상수, C# readonly
//require는 using하고 개념이 비슷하다.

//파일경로를 조립해줄 수 있는 모듈, 파일에 접근할수 있는 모듈
const fs = require('fs'); //파일시스템 모듈
const path = require('path'); //경로 모듈

const result = require('./result.js');

const server = http.createServer( function(request, response) {
    console.log(request.url, request.method);
    
    switch(request.url)
    {
        case "/":
            response.writeHead(200, {"Content-Type":"application/json"});
            response.end( JSON.stringify( result ));
            break;
        case "/image":
            //__dirname : 현재 폴더를 나타내는 상수
            let filePath = path.join(__dirname, "images", "youandme.png");
            //파일시스템에서 해당 파일의 정보를 가져온다.
            let fileStat = fs.statSync(filePath);

            response.writeHead(200, 
                {"Content-Type":"image/png", "Content-Length": fileStat.size});
            
            let readStream = fs.createReadStream(filePath);
            readStream.pipe(response);
            break;
    }
    
});

//일반적인 서버랑 다르게 웹은. 요청에 의한 응답만을 해
server.listen(52000, function(){
    console.log("Server is running on 52000");
});
