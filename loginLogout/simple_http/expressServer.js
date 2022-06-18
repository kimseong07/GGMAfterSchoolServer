const express = require('express');
const fs = require('fs/promises');
const http = require('http');
const path = require('path');
const {pool, insertData} = require('./DB');
const jwt = require('jsonwebtoken');

const key = "ggm";

const app = express();
// http://gondr.asuscomm.com/phpmyadmin 
// yy_401XX , 1234

app.use(express.json()); //이녀석은 요청을 json으로 변환해주는 역할


app.use( (req, res, next) => {
    console.log(req.headers);
    let auth = req.headers["authorization"];
    console.log(auth);
    if(auth == undefined){
        req.loginUser = null;
        next();
        return;
    }
    auth = auth.split(" "); //공백으로 문자열 쪼개주고
    let token = auth[1];
    

    if(token != undefined){
        const decode = jwt.verify(token, key); //해당 토큰이 해당키로 암호화 되었는지 체크
        if(decode)     {
            req.loginUser = decode;
        }else{
            res.json({result:false, payload:"변형된 토큰이 감지됨"});
            return;
        }
    }else{
        req.loginUser = null;
    }
    next();
});

//app 이 바로 요청이 왔을 때 응답을 해주는 함수야
const server = http.createServer(app);


app.get("/", (req, res)=>{
    res.json({msg:"메인페이지입니다."});
});

app.get("/hello", (req, res)=>{
    res.json({msg:"헬로 페이지입니다."});
});

app.get("/image", (req, res)=>{
    let filename = req.query.file;
    let filePath = path.join(__dirname, "images", filename);
    res.sendFile(filePath);
});

app.get("/record", (req, res)=>{
    console.log(req.query);

    res.json({msg:"당신의 기록 메세지로 대체되었다"});
});

app.get("/fileList", async (req, res) => {
    const files = await fs.readdir( path.join(__dirname, "images") );
    res.json({msg:"load success", list:files });
});

app.get("/thumb", async (req, res)=>{
    let filename = req.query.file;
    let filePath = path.join(__dirname, "thumbnails", filename);

    res.sendFile(filePath);
});

app.post("/postdata", async (req, res) =>{
    console.log(req.loginUser);
    if(req.loginUser != null){
        let {name, msg, score} = req.body;

        //로그인된 유저의 기록이 존재하는지 먼저 검사하고 
        //존재하면 insertData가 아니라 Update 로 score 만 갱신 . 단 이 때 기존 데이터보다 score가 
        // 클경우에만 갱신

        // UPDATE high_scores SET score = ? WHERE user = ?

        let result = await insertData(name, msg, score, req.loginUser.id );
        if(result){
            res.json({msg:"기록완료"});
        }else{
            res.json({msg:"기록중 오류 발생"});
        }
    }else{
        res.json({result:false, payload:"기록갱신은 로그인된 유저만 가능합니다."});
    }
});

app.get("/list", async (req, res)=>{
    
    if(req.loginUser != null){
        let sql = "SELECT * FROM high_scores ORDER BY score DESC LIMIT 0, 10";
        //시작 인덱스, 갯수
        let [list] = await pool.query(sql);    //배열로 나오고 0번째에는 1번째 컬럼정보 
        res.json({result:true, payload: JSON.stringify( {list, count:list.length}) });
    }else{
        res.json({result:false, payload:"잘못된 토큰입니다"});
    }
    
});

app.post("/register", async (req, res)=>{
    let {name, id, password} = req.body;
    
    let sql = `SELECT id FROM loginusers WHERE id = ?`;  //작성해서 입력한 id의 회원이 존재하는지 먼저
    // 검사
    let [user] = await pool.query(sql, [id]);
    if(user.length > 0){
        res.json({result:false, payload:"중복된 회원이 존재합니다."});
        return;
    }
    
    sql = `INSERT INTO loginusers (id, name, password) 
                VALUES (?, ?, PASSWORD(?))`;
    await pool.query(sql, [id, name, password]);

    res.json({result:true, payload:"성공적으로 회원가입"});
});

app.post("/login", async (req, res)=>{
    let {id, password} = req.body; 
    const [row] = await pool.query(
        "SELECT * FROM loginusers WHERE id = ? AND password = PASSWORD(?)",
        [id, password]);
    
    if(row.length > 0){
        let {code, id, name} = row[0];

        const token = jwt.sign({code, id, name}, key, {
            expiresIn:'30 days'
        });
        
        res.json({result:true, payload:token});
    }else{
        res.json({result:false, payload:"존재하지 않는 회원입니다."});
    }
    //jwt.io
});

server.listen(54000, ()=>{
    console.log("서버가 54000번 포트에서 구동중입니다.");
});

