const sharp = require('sharp');
const path = require('path');
const fs = require('fs/promises');



async function resizeFunc(targetDir) {
    let targetPath = path.join(__dirname, targetDir);  //현재경로/images

    let files = await fs.readdir(targetPath);

    for(let i = 0; i < files.length; i++){
        
        let file = path.join(targetPath, files[i]);
        let destPath = path.join(__dirname, "thumbnails", "r_" + files[i]);

        let info = await sharp(file).resize(100, 80).toFile(destPath);
    }    
}


resizeFunc("images");

