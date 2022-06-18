const secret = require('./secret'); 

const mysql = require('mysql2');
//접속정보를 기반으로 연결 풀을 만든다.
const pool = mysql.createPool(secret);
const promisePool = pool.promise(); //프로미스 기반의 풀을 만든다.


async function insertData(name, msg, score, id)
{
    let sql = `INSERT INTO high_scores 
                (name, msg, score, user) 
                VALUES (?, ?, ?, ?)`;
    let result = await promisePool.query(sql, [name, msg, score, id]);
    
    return result[0].affectedRows == 1;
}

module.exports = {
    pool:promisePool,
    insertData
};