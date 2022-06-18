function R(num)
{
    return new Promise((res, rej)=>{
        setTimeout(()=>{
            console.log(num);
            res();
        }, Math.random() * 2000);
    });
}

async function test(){
    for(let i = 0; i < 10; i++)
    {
        await R(i);
    }
}

test();
