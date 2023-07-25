import { getDatabase, ref, set } from "https://www.gstatic.com/firebasejs/10.1.0/firebase-database";
import { app } from "./AppConfig.js";


function writeBlueprintData(BlueprintId, BlueprintName, BlueprintDescription, UserId) {
    
    const database = getDatabase(app);

    set(ref(database, "blueprints/" + blueprintId), {
        BlueprintName: BlueprintName,
        BlueprintDescription: BlueprintDescription,
        UserId: UserId
    });
}

writeBlueprintData("blueprint 1", "blueprint", "blueprint description", "user 1");