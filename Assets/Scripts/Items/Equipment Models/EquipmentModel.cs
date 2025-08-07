using UnityEngine;

[CreateAssetMenu(menuName = "Equipment Model")]
public class EquipmentModel : ScriptableObject
{
    //[SerializeField] EquipmentModelType equipmentModelType; 
    public string equipmentModelName;


    public void LoadModel(PlayerManager player)
    {


    //    switch (equipmentModelType)
    //    {
    //        case EquipmentModelType.FullHelmt:

    //            foreach (var model in player.playerEquipmentManager.headFullHelmets)
    //            {
    //                if (model.gameObject.name == equipmentModelName)
    //                {
    //                    model.SetActive(true);
    //                    // if you want to assign a material (model.gameobject.getcomponent<renderer>().material = Instantiate(equipmentMaterial);
    //                }
    //                else
    //                {
    //                    model.SetActive(false);
    //                }
    //            }
    //            break;
    //        case EquipmentModelType.OpenHelmt:
    //            break;
    //        case EquipmentModelType.Hood:
    //            break;
    //        case EquipmentModelType.HelmetAcessorie:
    //            break;
    //        case EquipmentModelType.FaceCover:
    //            break;
    //        case EquipmentModelType.Torso:
    //            break;
    //        case EquipmentModelType.Back:
    //            break;
    //        case EquipmentModelType.RightShoulder:
    //            break;
    //        case EquipmentModelType.RightUpperArm:
    //            break;
    //        case EquipmentModelType.RightElbow:
    //            break;
    //        case EquipmentModelType.RightLowerArm:
    //            break;
    //        case EquipmentModelType.RightHand:
    //            break;
    //        case EquipmentModelType.LeftShoulder:
    //            break;
    //        case EquipmentModelType.LeftUpperArm:
    //            break;
    //        case EquipmentModelType.LeftElbow:
    //            break;
    //        case EquipmentModelType.LeftLowerArm:
    //            break;
    //        case EquipmentModelType.LeftHand:
    //            break;
    //        case EquipmentModelType.Hips:
    //            break;
    //        case EquipmentModelType.HipsAttachment:
    //            break;
    //        case EquipmentModelType.RightLeg:
    //            break;
    //        case EquipmentModelType.RightKnee:
    //            break;
    //        case EquipmentModelType.LeftLeg:
    //            break;
    //        case EquipmentModelType.LeftKnee:
    //            break;
    //    }
    }
}
