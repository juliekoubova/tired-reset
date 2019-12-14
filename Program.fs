open System
open Rest

let validatePutKey entityKey resource: RestResource<'Key, 'Entity> =
  function
  | Put (key, entity) as req ->
      let keyFromEntity = entityKey entity
      if key = keyFromEntity then
        resource req
      else
        PutFail ((RestFail.cannotChangeKey key keyFromEntity), key, entity)
  | req -> resource req

let convertKey resource convert convertBack: RestResource<'Key, 'Entity> =
 fun req ->
    req
    |> RestRequest.mapKey convert
    |> resource
    |> RestResult.mapKey convertBack

type Pet = {
  Name : String
  Owner : String
}

let petKey pet = pet.Name
let pets = InMemory.create petKey |> validatePutKey petKey

[<EntryPoint>]
let main _ =
  printfn "POST: %A" (pets <| Post { Name = "Moan"; Owner = "Daddy" })
  printfn "LIST: %A" (pets <| List)
  printfn "PUT: %A" (pets <| Put ("Moan", { Name = "Penny"; Owner = "Daddy" }))
  0