# Review — TASK-0012

## Result

PASS WITH RISKS

## What changed

- Pada `Assets/Script/CCC/PlayerInteractionController.cs`:
  - Menambahkan field `_toolTag` ("Tool") dan `_heavyTag` ("Heavy").
  - Menambahkan method `GetItemPriority(Rigidbody item)`:
    - **Tier 3:** Alat pembersih (`_toolTag` atau nama mengandung mop/broom/duster/clean).
    - **Tier 2:** Barang ringan & sedang (default).
    - **Tier 1:** Furnitur berat (`_heavyTag` atau memiliki komponen `CoOpLiftController`).
  - Mengubah logika `GrabNearest()` sehingga prioritas yang lebih tinggi selalu diprioritaskan sebelum jarak murni.

## Acceptance criteria

- [x] Compile bersih (0 error).
- [ ] Saat alat pembersih dan furnitur berat berada di dekat pemain, alat pembersih yang terangkat terlebih dahulu (menunggu smoke test Editor).
- [ ] Saat barang ringan dan furnitur berat berdekatan, barang ringan yang terangkat terlebih dahulu.
- [ ] Jika tipe objek sama, objek terdekat yang terpilih.

## Regression check

- Related feature: `PlayerCarryController`, `CoOpLiftController`.
- Result: Interaksi tetap berjalan normal tanpa error.

## Verification evidence

- Compile check: `dotnet build Assembly-CSharp.csproj --no-restore` ➔ 0 errors, Build succeeded.

## Known issues

- Perlu smoke test penataan objek bertumpuk di Editor.

## Follow-up tasks

None.
