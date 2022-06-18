let connectionData = {
    host:"gondr.asuscomm.com",
    user:"yy_40101",
    password:"1234",
    database:"yy_40101",
    timezone:"+09:00"
};
const mysql = require('mysql2');
//접속정보를 기반으로 연결 풀을 만든다.
const pool = mysql.createPool(connectionData);
const promisePool = pool.promise(); //프로미스 기반의 풀을 만든다.


async function insertData(name, msg, score)
{

    let sql = `INSERT INTO high_scores 
                (name, msg, score) 
                VALUES (?, ?, ?)`;
    let result = await promisePool.query(sql, [name, msg, score]);
    
    console.log(result);
}


insertData("게마고", "내가 만들고 싶은 게임을 만들자", 1200);
// SELECT * FROM high_scores ORDER BY score DESC, id DESC LIMIT 0, 5
async function getList()
{
    let sql = "SELECT name, msg, score FROM high_scores";
    let [rows] = await promisePool.query(sql);

    console.log(rows);
}

//getList();