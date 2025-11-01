// BulmacaSalteri.cs
using UnityEngine;
using UnityEngine.UI;

public class BulmacaSalteri : MonoBehaviour
{
    public int benimIDm; // Bu þalterin kimliði (0, 1, 2 veya 3)
    public SifreBeyniKontrol patronum; // Patronun kim olduðunu buraya sürükleyeceðiz

    // Button component'i týklandýðýnda bu fonksiyonu çaðýracak
    public void Tiklandim()
    {
        // Tek görevi patrona haber vermek: "Hey, bana týklandý! Benim ID'm bu."
        patronum.BirSaltereBasildi(benimIDm);
    }

    // Patron bu fonksiyonu çaðýrarak resmini deðiþtirmesini emreder
    public void ResminiDegistir(Sprite yeniResim)
    {
        GetComponent<Image>().sprite = yeniResim;
    }
}